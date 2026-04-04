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

    public List<GameObject> screws_projectFile;
    public List<GameObject> screws_blackoutHandle;
    public GameObject currentScrew;

    public KeyCode dropWrench = KeyCode.V;
    public KeyCode grabWrench = KeyCode.L;

    private bool clickAgain = false;

    public GameObject plate_projectFile;
    public GameObject plate_handle;

    private bool showControls = true;

    public AudioSource pickup;
    public AudioSource putdown;

    // test
    private float lastAngle;
    private float totalRotation = 0f;

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
        if (Input.GetKeyDown(dropWrench) && isHoldingWrench && !OnScrew)
        {
            DropWrench();
        }
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) && isHoldingWrench && OnScrew)
        {
            HoldWrench();
        }


        checkScrews(screws_projectFile, plate_projectFile);  // prob shouldn't call this constantly
        checkScrews(screws_blackoutHandle, plate_handle);  // prob shouldn't call this constantly
    }

    private void OnMouseDown()
    {
        if (!isHoldingWrench) 
        {
            pickup.Play();
            HoldWrench(); 
        }

        // for rotation stuffs
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - screenPos;

        lastAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void OnMouseDrag()
    {
        if (OnScrew && !clickAgain)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            Vector3 dir = Input.mousePosition - screenPos;
            float currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);

            if (deltaAngle >= 0)
            {
                totalRotation += deltaAngle;
            }
            else if (deltaAngle < 0)
            {
                totalRotation += deltaAngle;
            }

            if (totalRotation >= 360f)
            {
                //transform.localRotation = Quaternion.Euler(0f, 150f, 0f);
                currentScrew.GetComponent<Screw>().loosen();

                totalRotation = 0f;
            }
            else if (totalRotation <= -360f)
            {
                currentScrew.GetComponent<Screw>().tighten();

                totalRotation = 0f;
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
        else
        {
            putdown.Play();
        }
    }


    public void Unscrew()
    {
        // Rotates the wrench to unscrew a screw

        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - screenPos;

        float currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);

        transform.Rotate(Vector3.forward, -deltaAngle * 1f);

        lastAngle = currentAngle;
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

    public void setClickAgain(bool val)
    {
        clickAgain = val;
    }

    // ================== Unscrew Event ===================

    public void checkScrews(List<GameObject> scr, GameObject plt)
    {
        foreach (GameObject scrw in scr)
        {
            if (scrw.activeSelf)  // if ANY screw is active, still needs to be removed
            {
                return;
            }
        }

        plt.transform.localPosition = new Vector3(-3.3f, -3.05f, 0f);
    }

}
