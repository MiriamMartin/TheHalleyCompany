using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Gauge : MonoBehaviour, ButtonInterface, BlackoutInterface
{

    public bool DEBUGMODE = false;

    //Speeds
    private float speed;
    public float gaugeSpeed = 3f;
    public float maxSpeed = 3f;

    public float handleWhenPressed = 50f; //How much the handle get turned back
    public Vector3 forwardDirection = new Vector3();
    private Vector3 backwardDirection;
    private Vector3 currDirection;
    public float angleMin = -90;
    public float angleMax = 90;
    public float angleDangerMax = 50;
    public float angleDangerMin = -50;
    public float angleSteam = 0;
    private float currAngle;
    private bool handlePressed = false;
    
    //Light bulb stuff
    public GameObject bulb;
    private bool blinking = false;
    private bool blink;
    private Material mat;

    //Steam stuff
    public ParticleSystem steam;
    public float steamIntensityStart = 15f;
    public float steamIntensityMult = 5f;
    public AudioSource steamAudioSource;
    public AudioSource steamWhistleAudioSource;
    [Range(0f, 1f)]
    public float whistleMaxVolume;
    [Range(0f, 1f)]
    public float steamMaxVolume;


    private bool run = false;

    public GameObject needle;

    [Header("Handle")]
    public GameObject handle;
    private Vector3 handleDirection = new Vector3(0, -1, 0);
    public AudioSource handleAudioSRC;
    private Renderer handleRend;
    private bool onScreen = false;
    private Quaternion handleInitRot;

    private bool Paused = false;

    private bool canPress = true;

    private bool postBlackout = false;

    private void Start()
    {
        canPress = true;
        postBlackout = false;
        backwardDirection = -forwardDirection;
        currDirection = forwardDirection;
        speed = gaugeSpeed;
        Renderer rend = bulb.GetComponent<Renderer>();
        mat = rend.material;
        mat.DisableKeyword("_EMISSION");
        blink = false;
        blinking = false;
        steam.startSpeed = (steamIntensityStart + (0 * steamIntensityMult));
        steam.startColor = new Color(1f, 1f, 1f, 0);
        steamAudioSource.volume = 0;
        steamWhistleAudioSource.volume = 0;

        //Start randomizing speed
        StartCoroutine(RandomizeSpeed());


        Run();
        


        handleRend = handle.GetComponent<Renderer>();  // this is used to only allow holding while handle is on screen
        handleInitRot = handle.transform.rotation;
    }

    IEnumerator RandomizeSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 15));
            speed = Random.Range(0.5f, maxSpeed);

        }
    }

    public void Run()
    {
        run = true;
    }

    private void Update()
    {
        //checkVisibility();
        //checkUnpaused();

        if (Depth.Instance.runGauges && run)
        {
            currAngle = needle.transform.eulerAngles.z;
            //coverting from 0->360 to -180->180
            if (currAngle > 180f)
            {
                currAngle -= 360f;
            }

            if (handlePressed)
            {
                currDirection = backwardDirection;
            } else
            {
                currDirection = forwardDirection;
            }

            needle.transform.Rotate(speed * currDirection * Time.deltaTime); //This is the code that rotates

            if (currAngle > angleMax || currAngle < angleMin)
            {
                run = false;
                PauseManager.Instance.Death();
            }
            else if ((currAngle < angleDangerMin) && !blinking && (currAngle > angleMin)) 
            {
                InvokeRepeating("Blink", 0f, 0.2f);
                blinking = true;
            }
            else if ((currAngle > angleDangerMax) && !blinking && (currAngle < angleMax))
            {
                InvokeRepeating("Blink", 0f, 0.2f);
                blinking = true;
            } 
            else if (((currAngle <= angleDangerMax) && (currAngle >= angleDangerMin)) && blinking)
            {
                CancelInvoke(nameof(Blink));
                blinking = false;
                mat.DisableKeyword("_EMISSION");
            }

            //Post blackout
            if (postBlackout && (currAngle > (angleDangerMax + 10) || currAngle < (angleDangerMin - 10))) {
                speed = 0;
            }
            else if (postBlackout)
            {
                speed = maxSpeed * 5;
            }

            //Steam

            float steamIntensity = Mathf.InverseLerp(angleSteam, angleMax, Mathf.Abs(currAngle));
            steam.startSpeed = (steamIntensityStart + (steamIntensity * steamIntensityMult));
            steam.startColor = new Color(1f, 1f, 1f, steamIntensity);
            steamAudioSource.volume = (steamIntensity * steamMaxVolume);
            if (steamIntensity > 0.7)
            {
                float whistleIntensity = Mathf.InverseLerp(0.7f, 1f, steamIntensity);

                steamWhistleAudioSource.volume = (whistleIntensity * whistleMaxVolume);
            }
            else
            {
                steamWhistleAudioSource.volume = 0;
            }
           


        }
 
    }

    private void Blink()
    {
        if (blink)
        {
            mat.DisableKeyword("_EMISSION");
            blink = false;
        }
        else
        {
            mat.EnableKeyword("_EMISSION");
            blink = true;
        }

    }

    public void Button(bool mouseDown, string message)
    {

        if (!PauseManager.Instance.getIsPaused() && canPress && !InteractManager.Instance.getIsInteracting())
        {
            if (mouseDown)
            {
                handleAudioSRC.Play();
                if (!handlePressed)
                {
                    handlePressed = true;
                    handle.transform.Rotate(handleWhenPressed * handleDirection);
                }
                else
                {
                    handlePressed = false;
                    handle.transform.Rotate(handleWhenPressed * -handleDirection);

                }
            }
        }
    }

    // ======================= My two bugfixes, quarantined atm cause prob better fix =========================
    public void checkVisibility()  // note to future self, might be able to add this into button script so works for everything
    {
        // checks if handle is visible to the player, else turns it off

        if (handleRend.isVisible && !onScreen)
        {
            onScreen = true;
        }
        if (!handleRend.isVisible && onScreen)
        {
            resetHandle();
            onScreen = false;
        }
    }
    public void resetHandle()
    {
        // if handle not seen, stop it

        speed = gaugeSpeed;
        currDirection = forwardDirection;

        if (handlePressed) 
        { 
            handleAudioSRC.Play(); // only plays if handle was being pressed
            handle.transform.rotation = handleInitRot;  // puts handle back to off pos
        } 

        handlePressed = false;

    }

    public void checkUnpaused()
    {
        // when player unpauses the game, reset handle

        if (PauseManager.Instance.getIsPaused() && !Paused)
        {
            Paused = true;
        }
        if (!PauseManager.Instance.getIsPaused() && Paused)
        {
            Paused = false;
            resetHandle();
        }
    }

    // ======================================================================================

    public void BlackoutStart()
    {
        StartCoroutine(BlackoutStartCR());
    }

    IEnumerator BlackoutStartCR()
    {
        Debug.Log("BlackoutStartGauge");
        canPress = false;
        run = false;
        speed = gaugeSpeed * 2;
        float steamStartVolume = steamAudioSource.volume;
        float whistleStartVolume = steamWhistleAudioSource.volume;
        float colorAlphaStart = steam.startColor.a;
        float t = 0f;
        float duration = 5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            steamAudioSource.volume = Mathf.Lerp(steamStartVolume, 0, (t / duration));
            steamWhistleAudioSource.volume = Mathf.Lerp(whistleStartVolume, 0, (t / duration));
            float newAlpha = Mathf.Lerp(colorAlphaStart, 0, (t / duration));
            steam.startColor = new Color(1f, 1f, 1f, newAlpha);
            yield return null;
        }

        steamAudioSource.volume = 0;
        steamWhistleAudioSource.volume = 0;
        steam.startColor = new Color(1f, 1f, 1f, 0);
        Debug.Log("BlackoutStartGauge FINISHED!");
    }

    public void BlackoutEnd()
    {
        Debug.Log("BlackoutEndGauge");
        run = true;
        postBlackout = true;
    }

    public void CrazyTime()
    {
        canPress = true;
        postBlackout = false;
        speed = gaugeSpeed;
    }
}
