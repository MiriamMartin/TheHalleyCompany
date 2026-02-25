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
    public bool runBlackout = false;
    public bool runHitFloor = false;
    public bool runEnding = false;
    private float gaugeDepth = 400f; 
    private float switchDepth = 2100f;
    private float blackoutDepth = 3700f;
    private float maxDepth = 4400f;  // controls ending

    [Header("Blackout Event")]
    public BlackoutEvent blackoutEvent;

    [Header("Ending")]
    public bool ResetHandle = false;
    public Ending ending;  // will change this, just don't have time rn

    [Header("Radio Exposition")]
    public bool radioTrigger = false;
    public List<float> radioTriggerDepths = new List<float>();  
    private int radioTriggerIndex = 0;  // radio message 2 is first trigger (message 1 starts auto when tune in)
    public bool firstRadioDone = false;  // is first radio message done?

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
            AudioHandler.Instance.SetHallucinateInterval(60f);
        }
        if (depth >= switchDepth && (runSwitches == false) && !runBlackout)
        {
            runSwitches = true;
            AudioHandler.Instance.SetHallucinateInterval(45f);
        }
        if (depth >= blackoutDepth && (runBlackout == false))
        {
            runBlackout = true;
            runSwitches = false;
            descending = false;
            blackoutEvent.Run();
            AudioHandler.Instance.SetHallucinateInterval(20f);
        }
        if (depth >= maxDepth && (runEnding == false))
        {
            runHitFloor = true;
            runEnding = true;
            runSwitches = false;
            runGauges = false; // i don't think this is needed? but just extra assurance for the demo
            descending = false;
            blackoutEvent.Run();
            ending.Run();
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
}
