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

    public List<GameObject> screws_main;  // this is the one in the main room
    public List<GameObject> screws_blackoutHandle;  // one with the blackout handle
    public List<GameObject> screws_hallway;  // one in hallway with project files
    public GameObject currentScrew;

    public KeyCode dropWrench = KeyCode.V;
    public KeyCode grabWrench = KeyCode.L;

    private bool clickAgain = false;

    public GameObject plate_main;
    public GameObject plate_handle;
    public GameObject plate_hallway;

    private bool showControls = true;

    public AudioSource pickup;
    public AudioSource putdown;

    // test
    private float lastAngle;
    private float totalRotation = 0f;

    //Highlight
    public Renderer rend;
    private bool isHighlighted = false;

    private bool mainOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        ogPos = transform.localPosition;
        ogRot = transform.localRotation;
        rend.material.SetFloat("_ELevel", 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.Instance.getIsPaused() && isHighlighted)
        {
            highlightOff();
        }

        if (Input.GetKeyDown(dropWrench) && !PauseManager.Instance.getIsPaused() && isHoldingWrench)
        {
            if (!OnScrew)
            {
                DropWrench();
            }
            else
            {   
                isHoldingWrench = false;
                setCurrentScrew(null);
                DropWrench();
                HoldWrench();
            }
        }

        if (isHoldingWrench && (InteractManager.Instance.getIsInteracting() || InteractManager.Instance.getCanInteract() == false))
        {
            if (!OnScrew)
            {
                DropWrench();
            }
            else
            {
                isHoldingWrench = false;
                setCurrentScrew(null);
                DropWrench();
                HoldWrench();
            }
        }

        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) && isHoldingWrench && OnScrew)
        {
            if (OnScrew) { OnScrew = false; }
            HoldWrench();
        }

        
        if (!mainOpened) checkScrews(screws_main, plate_main, true);
        if (plate_handle.activeSelf) { checkScrews(screws_blackoutHandle, plate_handle, false); }
        if (plate_hallway.activeSelf) { checkScrews(screws_hallway, plate_hallway, false); }
    }

    private void OnMouseDown()
    {
        if (!isHoldingWrench && !PauseManager.Instance.getIsPaused() && !InteractManager.Instance.getIsInteracting() && InteractManager.Instance.getCanInteract()) 
        {
            pickup.Play();
            highlightOff();
            HoldWrench();
        }

        // for rotation stuffs
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - screenPos;

        lastAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
    }

    private void OnMouseDrag()
    {
        if (OnScrew && !clickAgain)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            Vector3 dir = Input.mousePosition - screenPos;
            float currentAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
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
                currentScrew.GetComponent<Screw>().tighten();

                totalRotation = 0f;
            }
            else if (totalRotation <= -360f)
            {
                currentScrew.GetComponent<Screw>().loosen();

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

        float currentAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);

        transform.Rotate(Vector3.forward, -deltaAngle * 1f);

        lastAngle = currentAngle;
    }

    // ===================== Highlight ====================
    public void OnMouseOver()
    {
        if (!PauseManager.Instance.getIsPaused() && !isHoldingWrench && !InteractManager.Instance.getIsInteracting() && InteractManager.Instance.getCanInteract())
        {
            highlightOn();
        }
    }

    public void OnMouseExit()
    {
        if (!PauseManager.Instance.getIsPaused())
        {
            highlightOff();
        }
    }
    private void highlightOn()
    {
        rend.material.SetFloat("_ELevel", 1.0f);
        isHighlighted = true;
    }

    private void highlightOff()
    {
        rend.material.SetFloat("_ELevel", 0.0f);
        isHighlighted = false;
    }


    // ================= Getters & Setters ================
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

    public void checkScrews(List<GameObject> scr, GameObject plt, bool main)
    {
        foreach (GameObject scrw in scr)
        {
            if (scrw.activeSelf)  // if ANY screw is active, still needs to be removed
            {
                return;
            }
        }

        if (main) 
        { 
            plt.transform.localPosition = new Vector3(-3.3f, -3.05f, 0f); 
            mainOpened = true;
        }
        else { plt.SetActive(false); }
    }
}
