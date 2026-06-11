using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BlackoutHandle : MonoBehaviour, ButtonInterface
{
    [Header("Blackout")]
    public List<MonoBehaviour> blackoutEffectedModules;
    public UnityEvent blackoutEnd;
    private bool pressed;

    [Header("Handle")]
    public GameObject handle;
    private Vector3 targetPos;
    private bool isRotating;

    [Header("Animation")]
    public Animator animator;

    [Header("Trailhead Demo?")]
    private bool DEMO = true;
    public GameObject end_script;

    public void Start()
    {
        pressed = false;
        isRotating = false;
        targetPos = new Vector3(90f, 0f, -90f);

        if (Checkpoints.Instance.getCheckpoint() >= 6)
        {
            animator.SetBool("hitLever", true); // plays the handle animation
        }

    }

    public void Update()
    {
        //HandleAnimation();
        //ResetHandle();  // better way to do this fs, will fix post demo
    }

    private void endBlackout()
    {
        foreach (MonoBehaviour blackoutObject in blackoutEffectedModules)
        {
            BlackoutInterface blackoutScript = blackoutObject as BlackoutInterface;
            blackoutScript.BlackoutEnd();
        }
            
    }

    public void Button(bool mouseDown, string message)
    {
        if (mouseDown && !pressed && !Depth.Instance.runEnding)  // can't turn everything on during ending
        {
            pressed = true;
            isRotating = true;
            animator.SetBool("hitLever", true); // plays the handle animation
            //Depth.Instance.runSwitches = true;
            //Depth.Instance.setDescending(true);
            Depth.Instance.FirstBlackoutDone = true;  // used only for terminal power
            if (DEMO)
            {
                end_script.GetComponent<Ending>().RUN_DEMO_ENDING();
            }
            else
            {
                blackoutEnd.Invoke();  // this is what it normally did pre-demo
            }
        }
    }

    public void HandleAnimation()
    {
        // animates the handle moving down after clicking

        if (isRotating && handle.transform.eulerAngles.x != targetPos.x)  // handle animation
        {
            //handle.transform.Rotate(0f, 0f, 0.5f, Space.World);
            //if (handle.transform.eulerAngles.x == targetPos.x) { isRotating = false; }
        }
    }

    public void ResetHandle()
    {
        // Resets handle back to og orientation (for 2nd blackout looks)

        if (Depth.Instance.ResetHandle)
        {
            handle.transform.Rotate(0f, 0f, -180f, Space.World);
            Depth.Instance.ResetHandle = true;
        }
    }
}
