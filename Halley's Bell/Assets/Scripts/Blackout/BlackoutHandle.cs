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

    public void Start()
    {
        pressed = false;
        isRotating = false;
        targetPos = new Vector3(90f, 0f, -90f);
    }

    public void Update()
    {
        HandleAnimation();
        ResetHandle();  // better way to do this fs, will fix post demo
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
            Depth.Instance.runSwitches = true;
            Depth.Instance.setDescending(true);
            blackoutEnd.Invoke();
        }
    }

    public void HandleAnimation()
    {
        // animates the handle moving down after clicking

        if (isRotating && handle.transform.eulerAngles.x != targetPos.x)  // handle animation
        {
            handle.transform.Rotate(0.5f, 0f, 0f, Space.World);
            if (handle.transform.eulerAngles.x == targetPos.x) { isRotating = false; }
        }
    }

    public void ResetHandle()
    {
        // Resets handle back to og orientation (for 2nd blackout looks)

        if (Depth.Instance.ResetHandle)
        {
            handle.transform.Rotate(-180f, 0f, 0f, Space.World);
            Depth.Instance.ResetHandle = true;
        }
    }
}
