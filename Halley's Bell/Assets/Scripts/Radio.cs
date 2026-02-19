using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Radio : MonoBehaviour, ButtonInterface
{
    const float minFreq = 88f;
    const float maxFreq = 108;
    public float range = 5f;
    public float triggerThreshold = 0.5f; //0.5 regular. FOR DEMO SET IT REALLY LOW TO LOCK IN
    public UnityEvent KeithTuned;

    //Button stuff
    private bool demoFrozen = false;
    private bool mouseDown;
    private string message;

    //Audio
    public AudioSource fuzz;
    public AudioSource a1;
    public AudioSource a2;
    public AudioSource keith;
    private AudioSource currAudioSource;

    //Logic
    private bool on = true;
    private bool switched = false;

    [Range(minFreq, maxFreq)]
    public float freq;
    public float increaseRate = 10;

    public Transform needle;
    private float needleZ;
    public float distanceMod = 1;


    //Speaker Light stuff
    public SpeakerLight speakerLight;

    // Start is called before the first frame update
    void Start()
    {
        Run();
        needleZ = needle.position.z;
        freq = minFreq;
    }

    void Run()
    {
        a1.volume = 0;
        a2.volume = 0;
        keith.volume = 0;
        fuzz.volume = 1;
        a1.Play();
        a2.Play();
        fuzz.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (on && !demoFrozen)
        {
            //Updating state of radioPoints (mostly audio for now) and checking if they are tuned in

            if (radioPoint(a1, 92))
            {
                Debug.Log("Tuned into a1");
                UpdateSpeakerLight();
            }
            else if (radioPoint(a2, 108))
            {
                Debug.Log("Tuned into a2");
                UpdateSpeakerLight();
            }
            else if (radioPoint(keith, 100))
            {
                Debug.Log("Tuned into Keith at at volume " + keith.volume);
                KeithTuned.Invoke(); //triggering intitial keith dialogue withy unity event
                keith.Play();

                //Demo code to freeze REMOVE POST DEMO
                if (demoTriggered(100))
                {
                    demoFrozen = true;
                }

                UpdateSpeakerLight();
            }
            else
            {
                speakerLight.disableLight();
                switched = false;
            }

            //Increasing or decreasing the freq over time depending on which button pressed   
            if (mouseDown && message == "increase")
            {
                freq += increaseRate * Time.deltaTime;
                freq = Mathf.Clamp(freq, minFreq, maxFreq); //Contraining the range
            }
            else if (mouseDown && message == "decrease")
            {
                freq -= increaseRate * Time.deltaTime;
                freq = Mathf.Clamp(freq, minFreq, maxFreq); //Contraining the range
            }
            else if (message == "power" && mouseDown)
            {
                message = "";
                on = false;
            }
            


            needle.position = new Vector3(needle.position.x, needle.position.y, needleZ + ((freq - minFreq) * distanceMod));
        }
        else if (mouseDown && !demoFrozen) //REMOVE DEMO FROZEN CONDITIONAL POST DEMO
        {
            a1.volume = 0;
            a2.volume = 0;
            keith.volume = 0;
            fuzz.volume = 0;
            if (message == "power")
            {
                Debug.Log("poweron!");
                fuzz.volume = 1;
                message = "";
                on = true;
            }
        }


    }

    //returns a float from 0-1 proportional to how close it is to the target frequency. (1 is on the freq, distance +/- range is 0)
    float getCloseness(float target)
    {
        if ((freq <= target + range) && (freq >= target - range))
        {
            return (1 - (Mathf.Abs(target - freq) / range));
        }
        return 0;
    }

    bool radioPoint(AudioSource audioSource, float target)
    {
        currAudioSource = audioSource;
        float closeness = getCloseness(target);
        audioSource.volume = closeness;
        if (closeness > 0)
        {
            fuzz.volume = (1 - closeness);
            if (closeness > triggerThreshold)
            {
                return true;
            }
        }
        return false;
    }

    public void Button(bool mouseDown, string message)
    {
        this.mouseDown = mouseDown;
        this.message = message;
    }

    private void UpdateSpeakerLight()
    {
        if (!switched)
        {
            speakerLight.setLightAudioSource(currAudioSource);
        }
        switched = true;
    }

    private bool demoTriggered(float freq)
    {
        float closeness = getCloseness(freq);
        return (closeness > 0.95);
    }


}
