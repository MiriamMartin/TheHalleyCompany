using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauge : MonoBehaviour, ButtonInterface, BlackoutInterface
{

    //Speeds
    private float speed;
    public float gaugeSpeed = 5f;
    public float handleSpeed = 10f;

    public float handlePower = 50f; //How much the handle get turned back
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

    public GameObject handle;
    private Vector3 handleDirection = new Vector3(0, -1, 0);

    public AudioSource audioSource;


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
    }

    public void Run()
    {
        run = true;
    }

    private void Update()
    {
        if (run)
        {
            currAngle = needle.transform.eulerAngles.z;
            //coverting from 0->360 to -180->180
            if (currAngle > 180f)
            {
                currAngle -= 360f;
            }

            if ((currAngle >= angleMin) || !handlePressed)
            {
                needle.transform.Rotate(speed * currDirection * Time.deltaTime);
            } 
            if (currAngle > 90)
            {
                Debug.Log("YOU DIED");
            }
            else if ((currAngle > angleDanger) && !blinking && (currAngle < angleMax))
            {
                Debug.Log("Guage Blinking");
                //audioSource.Play();
                InvokeRepeating("Blink", 0f, 0.2f);
                blinking = true;
            } 
            else if ((currAngle <= angleDanger) && blinking)
            {
                Debug.Log("Guage Stopped Blinking");
                CancelInvoke(nameof(Blink));
                //audioSource.Stop();
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
                Debug.Log("handle pressed! Direction: " + currDirection);
                currDirection = backwardDirection;
                handle.transform.Rotate(handlePower * handleDirection);
            }
            else
            {
                handlePressed = false;
                speed = gaugeSpeed;
                currDirection = forwardDirection;
                handle.transform.Rotate(handlePower * -handleDirection);
            }
        }
    }

    public void BlackoutEnd()
    {
        Debug.Log("GAUGE END BLAKOUT");
        return;
    }
}
