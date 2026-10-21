using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class newRadio : MonoBehaviour, ButtonInterface
{
    const float minFreq = 85f;
    const float maxFreq = 105;
    public float range = 1.5f;
    public float triggerThreshold = 0.5f; //0.5 regular. FOR DEMO SET IT REALLY LOW TO LOCK IN
    public UnityEvent KeithTuned;

    //Button stuff
    private bool demoFrozen = false;
    private bool mouseDown;
    private string message;

    //Audio
    public AudioSource fuzz;
    public AudioSource piano;
    public AudioSource currentEvents;
    public AudioSource music;
    public AudioSource scary;
    public AudioSource deepBlue;
    public AudioSource keith;
    private AudioSource currAudioSource;

    //Logic
    private bool on = false;
    private bool started = false;

    [Range(minFreq, maxFreq)]
    public float freq;
    public float increaseRate = 10;

    public Transform needle;
    private float needleZ;
    public float distanceMod = 1;

    [Header("Speaker Light")]
    public Material speakerBulbMat;
    public AudioLoudnessDetection ald;
    public Radio radio;  // not referenced, but removing breaks it? Not sure why lol. Same with OG script.
    public List<AudioSource> speakerLightSources;  // each of the sources (rn : [0] = bm_radio, [1] = keith)
    public AudioSource currSRC;  // the current audio source to play from
    public float intensity = 1.5f;
    private float currIntensity;
    [SerializeField] float lightBrightness;

    [Header("Dial Rotation")]
    public GameObject OneDial;

    [Header("Startup")]
    public bool radioConnected;

    void Start()
    {
        InitializeRadio();
        needleZ = needle.localPosition.z;
        freq = minFreq;
        started = false;

        speakerBulbMat.EnableKeyword("_EMISSION");

        // checkpoint stuff
        setNeedle();
        if (Checkpoints.Instance.getCheckpoint() == 0 || Checkpoints.Instance.getCheckpoint() >= 6) // turn off if starting or post-blackout
        {
            TurnOff();
        }
        else
        {
            radioConnected = true;
            TurnOn();
            on = true;
        }
    }

    void InitializeRadio()
    {
        // Initializes all radio vars at start

        piano.volume = 0;
        deepBlue.volume = 0;
        keith.volume = 0;
        piano.volume = 0;
        currentEvents.volume = 0;
        scary.volume = 0;
        music.volume = 0;
        fuzz.volume = 1;
        piano.Play();
        deepBlue.Play();
        currentEvents.Play();
        scary.Play();
        music.Play();
        fuzz.Play();
    }

    void Update()
    {
        SpeakerIntensity();  // update speaker intensity if needed

        if (on && !demoFrozen)
        {
            //Updating state of radioPoints (mostly audio for now) and checking if they are tuned in

            if (radioPoint(piano, 90))
            {
                SwapSpeakerLightSRC(speakerLightSources[0]);
            }
            else if (radioPoint(deepBlue, 92.5f))
            {
                SwapSpeakerLightSRC(speakerLightSources[1]);

            }
            else if (radioPoint(keith, 95) && Checkpoints.Instance.getCheckpoint() < 1)  // doesn't play 1st message if past that checkpoint
            {
                //Debug.Log("Tuned into Keith at at volume " + keith.volume);
                if (!started)
                {
                    Depth.Instance.tunedKeith = true;
                    keith.Play();
                    StartCoroutine(BellStartup());
                    started = true;
                }

                SwapSpeakerLightSRC(speakerLightSources[2]);
            }
            else if (radioPoint(music, 97.5f))
            {
                SwapSpeakerLightSRC(speakerLightSources[3]);

            }
            else if (radioPoint(currentEvents, 100))
            {
                SwapSpeakerLightSRC(speakerLightSources[4]);

            }
            else if (radioPoint(scary, 102.5f))
            {
                SwapSpeakerLightSRC(speakerLightSources[5]);

            }
            else
            {
                DisableSpeakerLight();
            }

            //  Increasing or decreasing the freq over time depending on which button pressed   
            if (OneDial.GetComponent<RadioDial>().GetRight())
            {
                freq += increaseRate * Time.deltaTime;
                freq = Mathf.Clamp(freq, minFreq, maxFreq); //  Constraining the range
                if (freq == maxFreq)
                {
                    OneDial.GetComponent<RadioDial>().SetStopRight(true);
                }
            }
            else if (OneDial.GetComponent<RadioDial>().GetLeft())
            {
                freq -= increaseRate * Time.deltaTime;
                freq = Mathf.Clamp(freq, minFreq, maxFreq); //  Constraining the range
                if (freq == minFreq)
                {
                    OneDial.GetComponent<RadioDial>().SetStopLeft(true);
                }
            }
            needle.localPosition = new Vector3(needle.localPosition.x, needle.localPosition.y, needleZ - ((freq - minFreq) * distanceMod));
        }

        if ((message == "power" && mouseDown) && radioConnected)
        {
            if (on) { TurnOff(); }
            else { TurnOn(); }
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

    private bool demoTriggered(float freq)  // What does this do?
    {
        float closeness = getCloseness(freq);
        return (closeness > (1 - triggerThreshold));
    }

    public void TurnOn()
    {
        fuzz.volume = 1;
        message = "";
        on = true;
    }

    public void TurnOff()
    {
        piano.volume = 0;
        deepBlue.volume = 0;
        keith.volume = 0;
        fuzz.volume = 0;
        piano.volume = 0;
        currentEvents.volume = 0;
        scary.volume = 0;
        message = "";
        on = false;
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

    public void PlayRadioDescentSeq(AudioClip radioClip)
    {
        keith.clip = radioClip;
        keith.Play();
    }

    public IEnumerator BellStartup()
    {
        // Bell only starts up when switches on AND radio message done

        yield return new WaitUntil(() => (!keith.isPlaying && !PauseManager.Instance.getIsPaused()));
        Depth.Instance.firstRadioDone = true;
    }

    public void BlackoutStart()
    {
        StartCoroutine(BlackoutStartCR());
    }

    IEnumerator BlackoutStartCR()
    {
        TurnOff();
        yield return new WaitForSeconds(0.2f);
        TurnOn();
        yield return new WaitForSeconds(0.6f);
        TurnOff();
    }

    // ============================= Checkpoint Stuff ================================

    public void setNeedle()
    {
        if (Checkpoints.Instance.getCheckpoint() >= 1)  // if radio was previous tuned into, start tuning AT keith frequency
        {
            needle.localPosition = new Vector3(2.410085f, 1.8464f, -1.1527f);
            freq = 95f;
        }
    }

    // =============================== TERMINAL ===============================

    public void ConnectRadio()
    {
        radioConnected = true;
        TurnOn();
    }
}

