using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioDial : MonoBehaviour
{

    public Camera mainCam;

    private float lastAngle;
    private bool turningRight = false;
    private bool turningLeft = false;

    private bool StopLeft = false;
    private bool StopRight = false;

    public float rotationSpeed = 1f;

    private void OnMouseDown()
    {
        // starts rotating from it's current direction (doesn't snap based on mouse pos)

        Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - screenPos;

        lastAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
    private void OnMouseDrag()
    {
        Rotate();
    }

    private void OnMouseUp()
    {
        turningLeft = false;
        turningRight = false;
    }

    public void Rotate()
    {
        // rotates based on mouse

        Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - screenPos;

        float currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);

        if ((deltaAngle > 0f && !StopLeft) || (deltaAngle < 0f) && !StopRight)
        {
            transform.Rotate(Vector3.forward, -deltaAngle * rotationSpeed);
        }
        

        lastAngle = currentAngle;

        if (deltaAngle > 0f)  // turning left (counter-clockwise)
        {
            turningLeft = true;
            turningRight = false;
            StopRight = false;
        }
        else if (deltaAngle < 0f)  // turning right (clockwise)
        {
            turningRight = true;
            turningLeft = false;
            StopLeft = false;
        }

        if (Mathf.Abs(deltaAngle) < 0.01f)  // if not dragging but mouse still held
        {
            turningLeft = false;
            turningRight = false;
            return;
        }
    }

    public bool GetLeft()
    {
        return turningLeft;
    }

    public bool GetRight()
    {
        return turningRight;
    }

    public void SetStopLeft(bool val)
    {
        StopLeft = val;
    }

    public void SetStopRight(bool val)
    {
        StopRight = val;
    }
}
