using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchEvent : MonoBehaviour
{

    public List<GameObject> switches;
    public AudioSource alarmAudio;
    public AudioSource switchOffAudio;
    public Material rlMat;

    public bool curRunning = false;
    public bool curWaiting = false;

    private float timer;
    private bool timerOn = false;

    private int minResetWait = 30;  // min number of seconds after successful event that next one is triggered
    private int maxResetWait = 50;  // max number of seconds after successful event that next one is triggered
    private float warnTime = 10f;    // alarm starts playing when this much time left                           
    private float failTime = 30f;   // how long from start of event before player has failed                    // NOTE: 25 feels good, made it 30 for extra room atm

    private bool startPowerOn = false;  // has the player turned them all on to start?
    private bool poweredOn = false; // used to only call it once

    private bool flashingLight = false;
    private bool PlayerFlipped = false;

    private IEnumerator Wait;
    private IEnumerator Light;

    private bool isFirstEvent = true;

    void Start()
    {
        rlMat.DisableKeyword("_EMISSION");

        // avoids null errors
        Light = RedLight();
        Wait = WaitForNextEvent();
    }

    void Update()
    {
        if (PauseManager.Instance.getIsPaused()) return;  // while paused, don't do or update anything

        PowerOn();  // check for initial power on to start descent

        if (Depth.Instance.runSwitches)  // while switch events are allowed to be active
        {
            checkPlayerOff();
            runSwitchEvent();
        }
        else if (curRunning && !Depth.Instance.runSwitches)  // turns off switch event during blackout
        {
            killSwitches();
            //turnOffSwitches();  // turns off lights during blackout
        }
    }

    public void PowerOn()
    {
        // Checks to see if player has powered on the ship at the beginning (aka, flipped all switches on).

        if (startPowerOn && Depth.Instance.firstRadioDone && !poweredOn)  // wait until player turns all switches on to start descent.
        {
            Depth.Instance.StartDescent();
            poweredOn = true;
            //startPowerOn = false; // only needs to be called once.
        }
        else if (!poweredOn)
        {
            startPowerOn = checkSwitchesComplete() ? true : false;  // once checkSC returns true, startPowerOn becomes true.
        }
    }

    public void runSwitchEvent()
    {
        // Handles the switch event

        if (!curRunning && !curWaiting && !PlayerFlipped) { runSwitches(); }  // if it's not already running, start it.
        else if (curRunning && !curWaiting)  // if it is running
        {   
            // Time Related Stuff
            if (timerOn) { updateTimer(); } // update timer first
            if (!flashingLight)
            {
                // Starts the Red Flashing Light once any switches are off

                flashingLight = true;
                Light = RedLight();
                StartCoroutine(RedLight());
            }
            if (timer < warnTime && timerOn && !alarmAudio.isPlaying) { alarmAudio.Play(); } // alarm / warning sound in last <warnTime> seconds

            // Event End Conditions
            if (checkSwitchesComplete() && timerOn)  // if timer's still on, and player succeeded
            {
                // Stop Flashing Light
                StopCoroutine(Light);
                rlMat.DisableKeyword("_EMISSION");

                // Reset for next event
                timerOn = false;
                curWaiting = true;
                curRunning = false;
                PlayerFlipped = false;
                Wait = WaitForNextEvent();
                StartCoroutine(Wait);
            }
            else if (!checkSwitchesComplete() && !timerOn)  // otherwise if timer's off and switches aren't done
            {
                curRunning = false;
                PauseManager.Instance.Death();
            }
        }
    }

    public void updateTimer()
    {
        // Counts down each frame, and turns timer off at 0.

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timerOn = false;
        }
    }

    public void resetTimer()
    {
        // Resets the timer to failTime, and indicates it's running
        
        if (isFirstEvent)
        {
            timer = 40f;  // gives player plenty of time on first incidence
            isFirstEvent = false;
        }
        else { timer = failTime; }
        
        timerOn = true;
    }

    public bool checkSwitchesComplete()
    {
        // Check if all switches are turned back on.

        int numOn = 0;
        int numSwitches = switches.Count;

        foreach (GameObject sw in switches)  // check each switch to see if it's on
        {
            if (sw != null && sw.GetComponent<Switch>().isSwitchOn())
            {
                numOn++;
            }
        }

        if (numOn == numSwitches) { return true; }  // if all switches are on, switches complete
        else { return false; }  // otherwise, switches not yet complete
    }
    public void runSwitches()
    {
        // starts a Switch Event

        curRunning = true;  // event is now running
        resetTimer();  // reset timer when event start
        int numOn = 0;

        foreach (GameObject sw in switches)  // for each switch, it tries a random light
        {
            if (sw != null)
            {
                sw.GetComponent<Switch>().RandomLight();
                if (sw.GetComponent<Switch>().isSwitchOn()) { numOn++; }
            }
        }

        if (numOn == switches.Count) { switches[0].GetComponent<Switch>().turnSwitchOff(); }  // makes sure atleast one is off


        switchOffAudio.Play();  // change this later, audio cue that switch is flipped off.
    }

    public IEnumerator WaitForNextEvent()
    {
        // Waits a random num of seconds in range minResetWait to maxResetWait before initiating another switch event.

        int randInt = UnityEngine.Random.Range(minResetWait, maxResetWait);  // wait b/w min & max seconds before triggering next event

        yield return new WaitForSeconds(randInt);
        curWaiting = false;
    }

    public IEnumerator RedLight()
    {
        // Flashes the red light when time getting low

        rlMat.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(1f);
        rlMat.DisableKeyword("_EMISSION");
        yield return new WaitForSeconds(1f);
        flashingLight = false;
    }

    public void checkPlayerOff()
    {
        // Checks if player turned one or more switches off, if so, starts event
        if (!checkSwitchesComplete() && !curRunning)
        {
            StopCoroutine(Wait);
            PlayerFlipped = true; 
            curWaiting = false;

            curRunning = true;
            resetTimer();  // reset timer when event start
        }
    }

    public void killSwitches()
    {
        // On blackout, ends whatever current switch event is going on.

        StopCoroutine(Wait);
        StopCoroutine(Light);
        resetTimer();
        //curRunning = false;
    }

    public void turnOffSwitches()
    {
        // turns off all switches on blackout(s)

        foreach (GameObject sw in switches)
        {
            if (sw != null)
            {
                sw.GetComponent<Switch>().turnSwitchOff();
            }
        }
    }

}
