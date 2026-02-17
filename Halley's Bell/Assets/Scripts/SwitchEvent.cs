using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SwitchEvent : MonoBehaviour
{

    public List<GameObject> switches;
    public AudioSource alarmAudio;
    public AudioSource switchOffAudio;

    public bool curRunning = false;
    public bool curWaiting = false;
    public float failTime;  // how long from start of event before player has failed

    private float timer;
    private bool timerOn = false;

    private int minResetWait = 5;  // min number of seconds after successful event that next one is triggered
    private int maxResetWait = 20; // max number of seconds after successful event that next one is triggered
    private float panicTime = 3f;  // alarm starts playing when this much time left

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.Instance.getIsPaused()) return;  // while paused, don't do or update anything

        if (Depth.Instance.runSwitches)  // while switch events are allowed to be active
        {
            if (!curRunning && !curWaiting) { runSwitches(); }  // if it's not already running, start it.
            else if (curRunning && !curWaiting)  // if it is running
            {
                if (timerOn) { updateTimer(); } // update timer first
                if (timer < 3f && timerOn && !alarmAudio.isPlaying) { alarmAudio.Play(); }  // alarm / warning sound in last 2 seconds
                if (checkSwitchesComplete() && timerOn)  // if timer's still on, and player succeeded
                {
                    timerOn = false;
                    curWaiting = true;
                    curRunning = false;
                    StartCoroutine(WaitForNextEvent());
                }
                else if (!checkSwitchesComplete() && !timerOn)  // otherwise if timer's off and switches aren't done
                {
                    curRunning = false;
                    Depth.Instance.Death();
                }
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

        timer = failTime;
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

        foreach (GameObject sw in switches)  // for each switch, it tries a random light
        {
            if (sw != null)
            {
                sw.GetComponent<Switch>().RandomLight();
            }
        }

        switchOffAudio.Play();  // change this later, audio cue that switch is flipped off. Could go off if rand event gives no switch tho.
    }

    public IEnumerator WaitForNextEvent()
    {
        // Waits a random num of seconds in range minResetWait to maxResetWait before initiating another switch event.

        int randInt = UnityEngine.Random.Range(minResetWait, maxResetWait);  // wait b/w min & max seconds before triggering next event

        yield return new WaitForSeconds(randInt);
        curWaiting = false;
    }

}
