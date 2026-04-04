using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depth : MonoBehaviour
{
    public static Depth Instance;

    [Header("Depth & Descent Settings")]
    public float descentSpeed = 1f;
    private float depth = 0f;

    [Header("Depth Indicator")]
    public Transform needle;

    [Header("Event Control")]
    public bool descending = false;
    public bool runGauges = false;
    public bool runSwitches = false;
    public bool runInhaler = false;
    public bool runBlackout = false;
    public bool runHitFloor = false;
    public bool runEnding = false;
    private float gaugeDepth = 600f; 
    private float switchDepth = 4400f;
    private float inhalerDepth = 6500f;
    private float blackoutDepth = 11800f;
    private float blackoutSafeDepth = 11900f;
    private float maxDepth = 15000f;  // controls ending

    [Header("Blackout Event")]
    public BlackoutEvent blackoutEvent;

    [Header("Hallucinationmaxxing Event")]
    public HalucinationMax halucinationMax; //yes, I spelled hallucination wrong!

    [Header("Ending")]
    public bool ResetHandle = false;
    public Ending ending;  // will change this, just don't have time rn

    [Header("Radio Exposition")]
    public bool radioTrigger = false;
    public List<float> radioTriggerDepths = new List<float>();  
    private int radioTriggerIndex = 0;  // radio message 2 is first trigger (message 1 starts auto when tune in)
    public bool firstRadioDone = false;  // is first radio message done?
    public bool tunedKeith = false;  // has first radio message been invoked?

    [Header("Hallucinations")]
    public bool canHallucinate = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (descending)
        {
            UpdateDepth();
            checkDepthEvents();
        }
    }

    public void StartDescent()
    {
        // When initial radio message done && switches all on, call this to start descent

        descending = true;
        this.GetComponent<AudioSource>().Play(); // plays start descent sound
        Checkpoints.Instance.updateCheckpoint(1);
    }
    public void UpdateDepth()
    {
        if (!PauseManager.Instance.getIsPaused() && depth <= maxDepth)
        {
            depth += (1000F/60F) * descentSpeed * Time.deltaTime;  // Depth changes by 1000 every 60 seconds, times descentSpeed (default = 1)
            needle.localPosition += Vector3.right * descentSpeed * Time.deltaTime * (1/1000f);  // standardized needle movement
        }
    }

    public void checkDepthEvents()
    {
        // At the given depth, set the run var for each event to true to trigger it.
        //
        // Each event's script should check to see if this is true in update,
        // calling run / allowing it to continue to run while this is true.
        //
        // When this is reset to false, it will stop.

        // Radio Triggering
        if (radioTriggerIndex < radioTriggerDepths.Count && (depth >= radioTriggerDepths[radioTriggerIndex]))
        {
            radioTrigger = true;
            radioTriggerIndex++;
        }

        // Actual Events
        if (depth >= gaugeDepth && (runGauges == false))
        {
            runGauges = true;
            canHallucinate = true; // start hallucinations post gauges
            AudioHandler.Instance.SetHallucinateInterval(60f);  // Delete These
            Checkpoints.Instance.updateCheckpoint(2);
        }
        if (depth >= switchDepth && (runSwitches == false) && !runBlackout)
        {
            runSwitches = true;
            AudioHandler.Instance.SetHallucinateInterval(45f);  // Delete These
            Checkpoints.Instance.updateCheckpoint(3);
        }
        if (depth >= inhalerDepth && (runInhaler == false))
        {
            runInhaler = true;
            Checkpoints.Instance.updateCheckpoint(4);
        }
        if (depth >= blackoutDepth && (runBlackout == false) && Checkpoints.Instance.getCheckpoint() <= 5) // only run blackout IF not passed it already
        {
            runBlackout = true;
            runSwitches = false;
            runInhaler = false;
            descending = false; 
            blackoutEvent.Run(); 
            AudioHandler.Instance.SetHallucinateInterval(20f);
            Checkpoints.Instance.updateCheckpoint(5);
        }
        if (depth >= blackoutSafeDepth && Checkpoints.Instance.getCheckpoint() < 6)
        {
            Checkpoints.Instance.updateCheckpoint(6);
            canHallucinate = false;
            halucinationMax.Run(); //Initialize hallucinationmaxxing
        }
        if (depth >= maxDepth && (runEnding == false))
        {
            runHitFloor = true;
            runEnding = true;
            runSwitches = false;
            runGauges = false; // i don't think this is needed? but just extra assurance for the demo
            runInhaler = false;
            descending = false;
            blackoutEvent.Run();
            ending.Run();
            Checkpoints.Instance.updateCheckpoint(7);
        }
    }

    // Getters & Setters
    public void setDescending(bool val)
    {
        descending = val;
    }

    public bool getDescending()
    {
        return descending;
    }

    public void setRadioIndex(int index)
    {
        radioTriggerIndex = index;
    }

    public void setDepth(float newDepth)
    {
        depth = newDepth;
    }
}
