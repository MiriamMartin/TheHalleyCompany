using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Gauge : MonoBehaviour, ButtonInterface, BlackoutInterface
{

    public bool DEBUGMODE = false;

    //Speeds
    private float speed;
    public float gaugeSpeed = 5f;
    public float handleSpeed = 10f;

    public float handleWhenPressed = 50f; //How much the handle get turned back
    public Vector3 forwardDirection = new Vector3();
    private Vector3 backwardDirection;
    private Vector3 currDirection;
    public float angleMin = -90;
    public float angleMax = 90;
    public float angleDanger = 50;
    public float angleSteam = -30;
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


    private void Start()
    {
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

        if (DEBUGMODE)
        {
            Run();
        }


        handleRend = handle.GetComponent<Renderer>();  // this is used to only allow holding while handle is on screen
        handleInitRot = handle.transform.rotation;
    }

    public void Run()
    {
        run = true;
    }

    private void Update()
    {
        checkVisibility();
        checkUnpaused();

        if (Depth.Instance.runGauges || run)
        {
            currAngle = needle.transform.eulerAngles.z;
            //coverting from 0->360 to -180->180
            if (currAngle > 180f)
            {
                currAngle -= 360f;
            }

            if ((currAngle >= angleMin) || !handlePressed)
            {
                needle.transform.Rotate(speed * currDirection * Time.deltaTime); //This is the code that rotates
            }

            if (currAngle > angleMax)
            {
                run = false;
                PauseManager.Instance.Death();
            }
            else if ((currAngle > angleDanger) && !blinking && (currAngle < angleMax))
            {
                InvokeRepeating("Blink", 0f, 0.2f);
                blinking = true;
            } 
            else if ((currAngle <= angleDanger) && blinking)
            {
                CancelInvoke(nameof(Blink));
                blinking = false;
                mat.DisableKeyword("_EMISSION");
            }

            //Steam
            
            if (currAngle > angleSteam)
            {
                float steamIntensity = Mathf.InverseLerp(angleSteam, angleMax, currAngle);
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

        if (!PauseManager.Instance.getIsPaused())
        {
            if (mouseDown)
            {
                handlePressed = true;
                speed = handleSpeed;
                currDirection = backwardDirection;
                handle.transform.Rotate(handleWhenPressed * handleDirection);
            }
            else if (handlePressed)
            {
                handlePressed = false;
                speed = gaugeSpeed;
                currDirection = forwardDirection;
                handle.transform.Rotate(handleWhenPressed * -handleDirection);
                handleAudioSRC.Play();
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
        currDirection = backwardDirection;
        speed = gaugeSpeed * 2; //stops speed to give player a chance to get back to their seat
        yield return new WaitUntil(() => {
            currAngle = needle.transform.eulerAngles.z;
            //coverting from 0->360 to -180->180
            if (currAngle > 180f)
            {
                currAngle -= 360f;
            }

            return currAngle <= angleMin;
        });
        speed = 0;
        run = false;
    }
    IEnumerator BlackoutEndCR()
    {
        currDirection = forwardDirection;
        speed = gaugeSpeed * 40; //stops speed to give player a chance to get back to their seat
        run = true;
        yield return new WaitUntil(() => {
            currAngle = needle.transform.eulerAngles.z;
            //coverting from 0->360 to -180->180
            if (currAngle > 180f)
            {
                currAngle -= 360f;
            }

            return currAngle > (angleDanger + 15);
            });
        
        speed = 0;
    }

    public void BlackoutEnd()
    {
        StartCoroutine(BlackoutEndCR());
    }

    public void CrazyTime()
    {
        speed = gaugeSpeed;
    }
}
