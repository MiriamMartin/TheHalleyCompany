using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    public static InteractManager Instance;
    private bool isInteracting = false;
    //public PostProcessVolume PPVolume;
    public GameObject canvas;
    public Transform overlayObjects;

    public GameObject BlurEffect;

    // Start is called before the first frame update
    void Start()
    {
        BlurEffect.SetActive(false);
        canvas.SetActive(false);
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
            BlurEffect.SetActive(true);
            canvas.SetActive(true);

        }
        else
        {
            BlurEffect.SetActive(false);
            canvas.SetActive(false);
            foreach (Transform child in overlayObjects)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
