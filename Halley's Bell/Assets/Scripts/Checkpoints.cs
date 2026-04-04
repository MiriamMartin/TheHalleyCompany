using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public static Checkpoints Instance;

    public HalucinationMax hallucinationMax;

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
                StartCoroutine(CheckpointFour());
                break;
            case 5:  // Blackout Starts
                StartCoroutine(CheckpointFive());
                break;
            case 6:  // Blackout Complete
                StartCoroutine(CheckpointSix());
                break;
            case 7:  // Ending Starts
                StartCoroutine(CheckpointSeven());
                break;
        }
    }

    public void updateCheckpoint(int chkpt)
    {
        // Updates the current checkpoint

        if (chkpt == PlayerPrefs.GetInt("CurrentCheckpoint") + 1)  // only updates checkpoint if it's one past the current one (so only if new progress)
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
        Debug.Log("Loading Checkpoint 1 - Start Descent");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        Depth.Instance.StartDescent();
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointTwo()
    {
        // Starting Gauges
        Debug.Log("Loading Checkpoint 2 - Gauges");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work


        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        //Depth.Instance.setRadioIndex(1);  // skips first message
        Depth.Instance.setDepth(500f);  // starts players a few seconds BEFORE gauges
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointThree()
    {
        // Starting Switches
        Debug.Log("Loading Checkpoint 3 - Switches");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setRadioIndex(2);  // skips first TWO messages (Gauge, Fear)
        Depth.Instance.setDepth(3800f);  // starts players a few seconds BEFORE switches message
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointFour()
    {
        // Starting Inhaler
        Debug.Log("Loading Checkpoint 4 - Inhaler");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setRadioIndex(3);  // skips first THREE messages (Gauge, Fear, Switch)
        Depth.Instance.setDepth(5800f);  // starts players a few seconds BEFORE inhaler message
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointFive()
    {
        // Starting Blackout
        Debug.Log("Loading Checkpoint 5 - Blackout");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setRadioIndex(5);  // skips first FIVE messages (Gauge, Fear, Switch, Inhaler, Chat Sesh)
        Depth.Instance.setDepth(10800f);  // starts players a few seconds BEFORE blackout message
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointSix()
    {
        // After Blackout (WILL NEED TO ADD LIGHTS STUFF)
        Debug.Log("Loading Checkpoint 6 - Blackout Over");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work
        //Hallucinations
        hallucinationMax.Run();
        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setRadioIndex(6);  // skips ALL messages EXCEPT Hallucination one
        Depth.Instance.setDepth(11900f);  // starts players further down than blackout
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

    public IEnumerator CheckpointSeven()
    {
        // Starting Blackout
        Debug.Log("Loading Checkpoint 7 - Ending");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setRadioIndex(7);  // skips ALL messages
        Depth.Instance.setDepth(13800f);  // starts players a few seconds BEFORE ending
        Depth.Instance.firstRadioDone = true;
        Depth.Instance.tunedKeith = true;
    }

}
