using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depth : MonoBehaviour
{
    public static Depth Instance;

    [Header("Depth & Descent Settings")]
    public float maxDepth;
    public float descentSpeed = 1f;
    private float depth = 0f;

    [Header("Depth Indicator")]
    public Transform needle;
    private float needleX;

    [Header("Event Control")]
    public bool descending = false;
    public bool runGauges = false;
    public float gaugeDepth = 1000f;
    public bool runSwitches = false;
    public float switchDepth = 500f;

    [Header("Death")]
    public GameObject DeathScreenOverlay;

    [Header("testing purposes -- can remove later")]
    public bool resetDepth = false;
    public Vector3 initNeedlePos;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        initNeedlePos = needle.localPosition;  // testing, can remove
    }

    // Update is called once per frame
    void Update()
    {
        if (descending)
        {
            UpdateDepth();
            checkDepthEvents();
        }

        if (resetDepth)  // testing, can remove
        {
            depth = 0f;
            needle.localPosition = initNeedlePos;
            resetDepth = false;
        }
    }

    public void UpdateDepth()
    {
        if (!PauseManager.Instance.getIsPaused() && depth < maxDepth)
        {
            depth += (1000F/60F) * descentSpeed * Time.deltaTime;  // Depth changes by 1000 every 60 seconds, times descentSpeed (default = 1)
            needle.localPosition += Vector3.right * descentSpeed * Time.deltaTime * (1/1000f);  // standardized needle movement
        }
    }

    public void checkDepthEvents()
    {
        // At the given depth, set the run var for each event to true.
        //
        // Each event's script should check to see if this is true in update,
        // calling run / allowing it to continue to run while this is true.
        //
        // When this is false, it will stop (i.e. on blackout event, everything set
        // to false until restart, when all set to true again).

        if (depth >= gaugeDepth && (runGauges == false))
        {
            runGauges = true;
        }
        if (depth >= switchDepth && (runSwitches == false))
        {
            runSwitches = true;
        }

    }

    public void Death()
    {
        // What happens when the player dies.

        PauseManager.Instance.setIsPaused(true);  // for now, to stop all events and whatnot in progress.
        DeathScreenOverlay.SetActive(true);

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
