using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class newDepth : MonoBehaviour
{
    public static newDepth Instance;

    [Header("Depth & Descent Settings")]
    public float descentSpeed = 1f;
    private float depth = 0f;
    private float maxDepth = 15000f;  // controls ending

    [Header("Depth Indicator")]
    public TMP_Text DepthText;

    [Header("Event Control")]
    public bool descending = false;
    public bool runGauges = false;
    public bool runSwitches = false;
    public bool runInhaler = false;
    public bool runBlackout = false;
    public bool runHitFloor = false;
    public bool runEnding = false;

    [Header("NEW DEPTH SEQUENCING")]
    public DescentSequence DS;
    private int nextEventIndex = 0;
    private DepthEvent curEvent;
    private HashSet<string> triggeredEvents = new HashSet<string>();
    private bool evntFired = false;
    private bool mssgFired = false;
    public Radio radio;
    public Terminal terminal;

    [Header("Blackout Event")]
    public BlackoutEvent blackoutEvent;
    public bool FirstBlackoutDone = false;

    [Header("Hallucinationmaxxing Event")]
    public HalucinationMax halucinationMax; //yes, I spelled hallucination wrong!

    [Header("Ending")]
    public bool ResetHandle = false;
    public Ending ending;  // will change this, just don't have time rn

    [Header("Radio Exposition")]
    public bool firstRadioDone = false;  // is first radio message done?
    public bool tunedKeith = false;  // has first radio message been invoked?

    public AudioSource TMPA;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        curEvent = DS.events[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {   
            curEvent.TypeText(terminal);
            curEvent.TEMPAUDIO(TMPA);
            curEvent.BarkText(terminal);
            nextEventIndex += 1;
            curEvent = DS.events[nextEventIndex];
        }

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
            depth += (1000F / 60F) * descentSpeed * Time.deltaTime;  // Depth changes by 1000 every 60 seconds, times descentSpeed (default = 1)
            DepthText.text = Mathf.Floor(depth / 10).ToString();
        }
    }

    public void checkDepthEvents()
    {
        if (depth >= curEvent.EventTrigger && !evntFired)
        {
            evntFired = true;
            Debug.Log(curEvent.EventID + ": EVENT TRIGGERED.");
            CheckExtraConditions(curEvent.EventID);
        }
        if (depth >= curEvent.MessageTrigger && !mssgFired)
        {
            mssgFired = true;
            Debug.Log(curEvent.EventID + ": MSSG TRIGGERED.");

            curEvent.PlayAudio(radio);
            curEvent.TypeText(terminal);

            // Here would be curEvent.PlayAudio() & curEvent.TypeText()
        }
        if (mssgFired && evntFired && !triggeredEvents.Contains(curEvent.EventID))
        {
            Debug.Log(curEvent.EventID + ": COMPLETE, MOVING ON...");
            triggeredEvents.Add(curEvent.EventID);
            nextEventIndex += 1;
            curEvent = DS.events[nextEventIndex];
            evntFired = false;
            mssgFired = false;
        }
    }

    void CheckExtraConditions(string id)
    {
        switch (id)
        {
            case "E1": // Gauges
                runGauges = true;
                Checkpoints.Instance.updateCheckpoint(2);
                Debug.Log("   -> E001 Extra Conditions Complete.");
                break;

            case "E2": // Switches
                runSwitches = true;
                Checkpoints.Instance.updateCheckpoint(3);
                Debug.Log("   -> E002 Extra Conditions Complete.");
                break;

            case "E3": // Inhaler
                runInhaler = true;
                Checkpoints.Instance.updateCheckpoint(4);
                halucinationMax.Run(); //Initialize hallucinationmaxxing FOR FAST-DEMO ONLY
                Debug.Log("   -> E003 Extra Conditions Complete.");
                break;

            case "E4": // Blackout
                runBlackout = true;
                runSwitches = false;
                runInhaler = false;
                descending = false;
                blackoutEvent.Run();
                Checkpoints.Instance.updateCheckpoint(5);
                Debug.Log("   -> E004 Extra Conditions Complete.");
                break;

            case "E5":  // Safe Post-Blackout
                Checkpoints.Instance.updateCheckpoint(6);
                halucinationMax.Run(); //Initialize hallucinationmaxxing
                break;

            case "E6":  // Ending
                runHitFloor = true;
                runEnding = true;
                runSwitches = false;
                runGauges = false; // i don't think this is needed? but just extra assurance for the demo
                runInhaler = false;
                descending = false;
                blackoutEvent.Run();
                ending.Run();
                Checkpoints.Instance.updateCheckpoint(7);
                break;
        }
    }

    // Getters & Setters
    public void setDescending(bool val)
    {
        descending = val;
    }

    public void setDepth(float newDepth)
    {
        depth = newDepth;
    }
    public float getDepth()
    {
        return depth;
    }
}
