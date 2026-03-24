using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;

public class Wrench : MonoBehaviour
{

    public Camera mainCamera;

    private Vector3 ogPos;
    private Quaternion ogRot;

    public bool isHoldingWrench = false;

    public bool OnScrew = false;

    private Vector3 lastMousePosition;

    public List<GameObject> screws;
    public GameObject currentScrew;

    public KeyCode dropWrench = KeyCode.V;
    public KeyCode grabWrench = KeyCode.L;

    private bool clickAgain = false;

    public GameObject plate;

    // Start is called before the first frame update
    void Start()
    {
        ogPos = transform.localPosition;
        ogRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(grabWrench) && !isHoldingWrench)
        {
            HoldWrench();
        }
        if (Input.GetKeyDown(dropWrench))
        {
            DropWrench();
        }


        checkScrews();  // prob shouldn't call this constantly
    }

    private void OnMouseDown()
    {
        if (!isHoldingWrench) { HoldWrench(); }

        // for rotation stuffs
        lastMousePosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (OnScrew && !clickAgain)
        {
            if (transform.localRotation.z >= 0.45)
            {
                transform.localRotation = Quaternion.identity;
                currentScrew.GetComponent<Screw>().increaseCranks();
                clickAgain = true;
            }
            else
            {
                Unscrew();
            }
        }
    }

    private void OnMouseUp()
    {
        clickAgain = false;
    }

    public void HoldWrench()
    {
        // pull wrench to side

        //this.transform.parent = mainCamera.transform;

        //this.transform.localPosition = new Vector3(0.4f, -0.15f, 1f);
        //this.transform.localRotation = Quaternion.Euler(0, 75, -15);


        transform.position = new Vector3(2.95f, -0.15f, 4.5f - 10f);  // need Y & Z to accomodate for camera's Y = 1 & Z = 10 (subtract fr each)
        transform.localRotation = Quaternion.Euler(20, 60, -30);

        transform.parent = mainCamera.transform;

        isHoldingWrench = true;

    }

    public void DropWrench()
    {
        // drop the wrench

        transform.parent = null;

        transform.localPosition = ogPos;
        transform.localRotation = ogRot;

        isHoldingWrench = false;

        if (OnScrew)
        {
            OnScrew = false;
        }
    }

    public void Unscrew()
    {
        // Rotates the wrench to unscrew a screw

        Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
        float rotationY = deltaMouse.y * 70 * Time.deltaTime;
        float rotationX = deltaMouse.x * -70 * Time.deltaTime;
        float totalRotation = rotationY + rotationX;

        transform.Rotate(Vector3.forward, totalRotation, Space.World);
        lastMousePosition = Input.mousePosition;
    }

    public void setOnScrew(bool val)
    {
        OnScrew = val;
    }

    public bool getOnScrew()
    {
        return OnScrew;
    }

    public bool GetHoldingWrench()
    {
        return isHoldingWrench;
    }

    public void setCurrentScrew(GameObject screw)
    {
        currentScrew = screw;
    }

    public string getCurrentScrew()
    {
        if (currentScrew != null) { return currentScrew.name; }
        else { return "noScrew"; }
    }

    // ================== Unscrew Event ===================

    public void checkScrews()
    {
        foreach (GameObject scrw in screws)
        {
            if (scrw.activeSelf)  // if ANY screw is active, still needs to be removed
            {
                return;
            }
        }

        plate.transform.localPosition = new Vector3(-3.3f, -3.05f, 0f);
    }

}
