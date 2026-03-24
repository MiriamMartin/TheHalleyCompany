using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public static Checkpoints Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        PlayerPrefs.SetInt("CurrentCheckpoint", 7);
        loadCheckpoint(PlayerPrefs.GetInt("CurrentCheckpoint", 0));  // loads current checkpoint, if no value exists yet loads nothing
    }

    public void loadCheckpoint(int chckpt)
    {
        // Loads the current checkpoint by altering depth values.

        switch (chckpt)
        {
            case 1:  // Bell Descent Starts
                StartCoroutine(CheckpointOne());
                break;
            case 2:  // Gauges Starts
                StartCoroutine(CheckpointTwo());
                break;
            case 3:  // Switches Starts
                StartCoroutine(CheckpointThree());
                break;
            case 4:  // Inhaler Starts

            case 5:  // Blackout Starts
                StartCoroutine(CheckpointFive());
                break;
            case 6:  // Backout Complete

            case 7:  // Ending Starts
                StartCoroutine(CheckpointSeven());
                break;
            case 8:
                break;
        }
    }

    public void updateCheckpoint(int chkpt)
    {
        // Updates the current checkpoint

        if (chkpt == PlayerPrefs.GetInt("CurrentCheckpoint") + 1)  // only updates checkpoint if it's one passed the current one (so only if new progress)
        {
            PlayerPrefs.SetInt("CurrentCheckpoint", chkpt);
        }
    }

    public int getCheckpoint()
    {
        return PlayerPrefs.GetInt("CurrentCheckpoint");
    }

    public IEnumerator CheckpointOne()
    {
        // starts the descent, turns switches on,
        Debug.Log("Loading Checkpoint 1");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        Depth.Instance.StartDescent();
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointTwo()
    {
        // Starting Gauges
        Debug.Log("Loading Checkpoint 2");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work


        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        //Depth.Instance.setRadioIndex(1);  // skips first message
        Depth.Instance.setDepth(350f);  // starts players a few seconds BEFORE gauges
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointThree()
    {
        // Starting Switches
        Debug.Log("Loading Checkpoint 3");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setRadioIndex(1);  // skips first message
        Depth.Instance.setDepth(1600f);  // starts players a few seconds BEFORE switches message
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointFive()
    {
        // Starting Blackout
        Debug.Log("Loading Checkpoint 5");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setRadioIndex(2);  // skips first two messages
        Depth.Instance.setDepth(2900f);  // starts players a few seconds BEFORE blackout message
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointSeven()
    {
        // Starting Blackout
        Debug.Log("Loading Checkpoint 7");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setRadioIndex(3);  // skips ALL messages
        Depth.Instance.setDepth(4200f);  // starts players a few seconds BEFORE blackout
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

}
