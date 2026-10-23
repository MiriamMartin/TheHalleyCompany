using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Depth : MonoBehaviour
{
    public static Depth Instance;

    [Header("Depth & Descent Settings")]
    public float descentSpeed = 1f;
    private float depth = 0f;

    [Header("Depth Indicator")]
    //public Transform needle;
    public TMP_Text DepthText;

    [Header("Event Control")]
    public bool descending = false;
    public bool runGauges = false;
    public bool runSwitches = false;
    public bool runInhaler = false;
    public bool runBlackout = false;
    public bool runHitFloor = false;
    public bool runEnding = false;
    private float maxDepth = 15000f;  // controls ending

    [Header("NEW DEPTH SEQUENCING")]
    public DepthEvents DS;
    public int nextEventIndex = 0;
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

    [Header("Hallucinations")]
    public bool canHallucinate = false; // ---------------------------------------------------------------------------------- Delete w/ AudioHandling Hallucinate?

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        curEvent = DS.events[nextEventIndex];
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

        this.GetComponent<AudioSource>().Play(); // plays start descent sound
        Checkpoints.Instance.updateCheckpoint(1);
        descending = true;
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
        if (mssgFired && evntFired && !triggeredEvents.Contains(curEvent.EventID))
        {
            queueNextEvent();
            evntFired = false;
            mssgFired = false;
        }
        else if (depth >= curEvent.EventTrigger && !evntFired)
        {
            evntFired = true;
            CheckExtraConditions(curEvent.EventID);
        }
        else if (depth >= curEvent.MessageTrigger && !mssgFired)
        {
            mssgFired = true;
            curEvent.PlayMessage(radio, terminal);
        }

    }

    void CheckExtraConditions(string id)
    {
        switch (id)
        {
            case "E1": // Gauges
                runGauges = true;
                canHallucinate = true; // start hallucinations post gauges
                AudioHandler.Instance.SetHallucinateInterval(60f);  // Delete These
                Checkpoints.Instance.updateCheckpoint(2);
                break;

            case "E2": // Switches
                runSwitches = true;
                AudioHandler.Instance.SetHallucinateInterval(45f);  // Delete These
                Checkpoints.Instance.updateCheckpoint(3);
                break;

            case "E3": // Inhaler
                runInhaler = true;
                Checkpoints.Instance.updateCheckpoint(4);
                break;

            case "E4": // Blackout
                if (Checkpoints.Instance.getCheckpoint() <= 5)
                {
                    runBlackout = true;
                    runSwitches = false;
                    runInhaler = false;
                    descending = false;
                    blackoutEvent.Run();
                    AudioHandler.Instance.SetHallucinateInterval(20f);
                    Checkpoints.Instance.updateCheckpoint(5);
                }
                break;


            case "E5":  // Safe Post-Blackout
                canHallucinate = false;
                Checkpoints.Instance.updateCheckpoint(6);
                halucinationMax.Run(); //Initialize hallucinationmaxxing
                FirstBlackoutDone = true;
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

    public void onCheckpointLoad(int chck)
    {
        for (int i = 0; i < chck; i++)
        {
            curEvent.addSavedText(terminal);
            CheckExtraConditions(curEvent.EventID);
            queueNextEvent();
        }
    }

    public void playIntro()
    {
        curEvent.PlayAudio(radio);
        curEvent.TypeText(terminal);
        queueNextEvent();
    }

    public void queueNextEvent()
    {
        triggeredEvents.Add(curEvent.EventID);
        nextEventIndex++;
        curEvent = DS.events[nextEventIndex];
    }

    // Getters & Setters
    public void setCheckpointDepth(int index)
    {
        depth = DS.events[index].CheckpointDepth;
    }
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
        var radioTriggerIndex = index;
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
