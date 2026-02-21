using System;
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

    [Header("Radio Message Sequencing")]
    public List<AudioClip> radioClips = new List<AudioClip>();
    private int radioClipIndex = 0;

    [Header("Speaker Light")]
    public Material speakerBulbMat;
    public AudioLoudnessDetection ald;
    public Radio radio;  // not referenced, but removing breaks it? Not sure why lol. Same with OG script.
    public List<AudioSource> speakerLightSources;  // each of the sources (rn : [0] = bm_radio, [1] = keith)
    public AudioSource currSRC;  // the current audio source to play from
    public float intensity = 1.5f;
    private float currIntensity;
    [SerializeField] float lightBrightness;

    // Start is called before the first frame update
    void Start()
    {
        InitializeRadio();
        needleZ = needle.localPosition.z;
        freq = minFreq;

        keith.clip = radioClips[0];
        radioClipIndex++;
        speakerBulbMat.EnableKeyword("_EMISSION");
    }

    void InitializeRadio()
    {
        // Initializes all radio vars at start

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
        CheckRadioTrigger();  // check for radio triggers from Depth
        SpeakerIntensity();  // update speaker intensity if needed

        if (on && !demoFrozen)
        {
            //Updating state of radioPoints (mostly audio for now) and checking if they are tuned in

            if (radioPoint(a1, 92))
            {
                //Debug.Log("Tuned into a1");
                SwapSpeakerLightSRC(speakerLightSources[0]);
            }
            else if (radioPoint(a2, 108))
            {
                //Debug.Log("Tuned into a2");
            }
            else if (radioPoint(keith, 100))
            {
                //Debug.Log("Tuned into Keith at at volume " + keith.volume);
                KeithTuned.Invoke(); //triggering initial keith dialogue with unity event
                keith.Play();
                StartCoroutine(BellStartup());


                //Demo code to freeze REMOVE POST DEMO
                if (demoTriggered(100))
                {
                    demoFrozen = true;
                }

                SwapSpeakerLightSRC(speakerLightSources[1]);
            }
            else
            {
                DisableSpeakerLight();
            }

            //  Increasing or decreasing the freq over time depending on which button pressed   
            if (mouseDown && message == "increase")
            {
                freq += increaseRate * Time.deltaTime;
                freq = Mathf.Clamp(freq, minFreq, maxFreq); //  Constraining the range
            }
            else if (mouseDown && message == "decrease")
            {
                freq -= increaseRate * Time.deltaTime;
                freq = Mathf.Clamp(freq, minFreq, maxFreq); //  Constraining the range
            }
            else if (message == "power" && mouseDown)
            {
                message = "";
                on = false;
            }

            needle.localPosition = new Vector3(needle.localPosition.x, needle.localPosition.y, needleZ - ((freq - minFreq) * distanceMod));
        }
        else if (mouseDown && !demoFrozen) //REMOVE DEMO FROZEN CONDITIONAL POST DEMO
        {
            a1.volume = 0;
            a2.volume = 0;
            keith.volume = 0;
            fuzz.volume = 0;
            if (message == "power")
            {
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

    private bool demoTriggered(float freq)
    {
        float closeness = getCloseness(freq);
        return (closeness > (1 - triggerThreshold));
    }

    // ============================= Speaker Light ================================
    private void SpeakerIntensity()
    {
        float colourMult = ald.GetLoudnessFromAudio(currSRC.timeSamples, currSRC.clip) * currIntensity;
        speakerBulbMat.SetColor("_EmissionColor", Color.yellow * colourMult * currIntensity);
        lightBrightness = ald.GetLoudnessFromAudio(currSRC.timeSamples, currSRC.clip) * currIntensity;
    }
    private void SwapSpeakerLightSRC(AudioSource newSRC)
    {
        currIntensity = intensity;
        currSRC = newSRC;
    }

    private void DisableSpeakerLight()
    {
        currIntensity = 0f;
    }

    // ============================= Radio Message Sequencing ================================

    public void CheckRadioTrigger()
    {
        // Checks for radio triggers from Depth, plays next message when triggered

        if (on && Depth.Instance.radioTrigger)  // Radio Trigger from Depth
        {
            Depth.Instance.radioTrigger = false;
            PlayRadioMessage();
        }
    }
    public void PlayRadioMessage()
    {
        // Plays the next queued radio message, and increases the index

        keith.clip = radioClips[radioClipIndex];
        radioClipIndex++;
        keith.Play();
    }

    public IEnumerator BellStartup()
    {
        // Bell only starts up when switches on AND radio message done

        yield return new WaitUntil(() => !keith.isPlaying);
        Depth.Instance.firstRadioDone = true;
    }

}
