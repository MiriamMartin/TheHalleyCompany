using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private InteractManager interactManager;
    public bool rotatable = false;
    public float distance = 1.5f;

    //Some code from YouTube tutorial: https://www.youtube.com/watch?v=_lEZ4PBw3Co&t=197s
    public GameObject objectToInspect;
    public float rotationSpeed = 10f;
    private Vector3 previousMousePosition;


    void Start()
    {
        interactManager = GetComponentInParent<InteractManager>();
        if (interactManager == null)
        {
            Debug.Log("Interactable object '" + gameObject.name + "' needs to be a child of interact manager object!");
        }
        
        if (objectToInspect == null)
        {
            Debug.Log("Must have object to inspect on interactable object '" + gameObject.name + "'");
        } else
        {
            objectToInspect.SetActive(false);

        }
    }

    private void Update()
    {
        if (rotatable)
        {
            //Code from tutorial to rotate object
            if (Input.GetMouseButtonDown(0) && !PauseManager.Instance.getIsPaused())
            {
                previousMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0) && !PauseManager.Instance.getIsPaused())
            {
                Vector3 deltaMousePosition = Input.mousePosition - previousMousePosition;
                float rotationX = deltaMousePosition.y * rotationSpeed * Time.deltaTime;
                float rotationY = -deltaMousePosition.x * rotationSpeed * Time.deltaTime;

                Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
                objectToInspect.transform.rotation = rotation * objectToInspect.transform.rotation;

                previousMousePosition = Input.mousePosition;
            } 
        }
    }


    private void OnMouseDown()
    {
        if (PauseManager.Instance.getIsPaused() || InteractManager.Instance.getIsInteracting()) { return; }  // Interactables can't be clicked while paused or already interacting
        interactManager.setIsInteracting(true);
        objectToInspect.SetActive(true);
        Transform cameraTransform = Camera.main.transform;
        objectToInspect.transform.position = cameraTransform.position + cameraTransform.forward * distance;
        objectToInspect.transform.rotation = cameraTransform.rotation;
    }

    private void OnMouseUp()
    {

    }
}
