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

    private bool showControls = true;

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

            Debug.Log(transform.localRotation.z);

            if (transform.localRotation.z <= -0.08)//transform.localRotation.z <= 0.45)
            {
                transform.localRotation = Quaternion.Euler(0f, 150f, 0f);
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

        transform.parent = mainCamera.transform;

        // Wrench Scene
        //this.transform.localPosition = new Vector3(1f, -0.15f, 2f);
        //this.transform.localRotation = Quaternion.Euler(40, 70, -30);

        // Unified Scene
        this.transform.localPosition = new Vector3(0.3f, 0f, 0.7f);
        this.transform.localRotation = Quaternion.Euler(-20, -95, 20);

        isHoldingWrench = true;

        if (showControls)
        {
            mainCamera.GetComponent<CameraMovement>().WrenchControls();
            showControls = false;
        }
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
