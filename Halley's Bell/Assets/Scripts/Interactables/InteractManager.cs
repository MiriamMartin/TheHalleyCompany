using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class InteractManager : MonoBehaviour
{
    public static InteractManager Instance;
    private bool isInteracting = false;
    public PostProcessVolume PPVolume;
    public GameObject canvas;
    public Transform overlayObjects;

    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);
        PPVolume.weight = 0;
        Instance = this;
        isInteracting = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool getIsInteracting()
    {
        return isInteracting;
    }

    public void setIsInteracting(bool val)
    {
        isInteracting = val;
        if (isInteracting)
        {
            PPVolume.weight = 1;
            canvas.SetActive(true);

        }
        else
        {
            PPVolume.weight = 0;
            canvas.SetActive(false);
            foreach (Transform child in overlayObjects)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
