using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public static Checkpoints Instance;

    public HalucinationMax hallucinationMax;

    public DescentSequence dseq;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        Depth.Instance.onCheckpointLoad(1);

    }

    public IEnumerator CheckpointTwo()
    {
        // Starting Gauges
        Debug.Log("Loading Checkpoint 2 - Gauges");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work
        Depth.Instance.descending = true;
        Depth.Instance.setCheckpointDepth(2);  // starts players a few seconds BEFORE gauges
        Depth.Instance.onCheckpointLoad(2);
        Depth.Instance.firstRadioDone = true;
    }

    public IEnumerator CheckpointThree()
    {
        // Starting Switches
        Debug.Log("Loading Checkpoint 3 - Switches");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setCheckpointDepth(4);
        Depth.Instance.onCheckpointLoad(4);
        Depth.Instance.firstRadioDone = true;
    }

    public IEnumerator CheckpointFour()
    {
        // Starting Inhaler
        Debug.Log("Loading Checkpoint 4 - Inhaler");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setCheckpointDepth(6);
        Depth.Instance.onCheckpointLoad(6);
        Depth.Instance.firstRadioDone = true;
    }

    public IEnumerator CheckpointFive()
    {
        // Starting Blackout
        Debug.Log("Loading Checkpoint 5 - Blackout");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setCheckpointDepth(10);
        Depth.Instance.onCheckpointLoad(10);
        Depth.Instance.firstRadioDone = true;
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
        Depth.Instance.setCheckpointDepth(11);
        Depth.Instance.onCheckpointLoad(11);
        Depth.Instance.firstRadioDone = true;
    }

    public IEnumerator CheckpointSeven()
    {
        // Starting Blackout
        Debug.Log("Loading Checkpoint 7 - Ending");

        yield return new WaitForSeconds(0.5f); // waits for all other scripts to initialize, else doesn't work

        // Need to update Depth Indicator Needle (!!)
        Depth.Instance.descending = true;
        Depth.Instance.setCheckpointDepth(16);
        Depth.Instance.onCheckpointLoad(16);
        Depth.Instance.firstRadioDone = true;
    }

}
