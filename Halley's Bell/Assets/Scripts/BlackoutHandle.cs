using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlackoutHandle : MonoBehaviour, ButtonInterface
{

    public List<MonoBehaviour> blackoutEffectedModules;
    public Vector3 direction = new Vector3();
    public float angle = 50;
    private bool pressed;
    public GameObject handle;
    public UnityEvent blackoutEnd;

    public void Start()
    {
        pressed = false;
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
        if (mouseDown && !pressed)
        {
            pressed = true;
            blackoutEnd.Invoke();
            //endBlackout();
            handle.transform.Rotate(angle * direction);
        }
    }

}
