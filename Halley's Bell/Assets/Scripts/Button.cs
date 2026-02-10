using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    public string message; //Allows for a message to be sent to the button method in case additional information needed other than "button pressed"
    private AudioSource audioSource;

    [SerializeField] private MonoBehaviour target; //Created because interfaces can't show up in the inspector for some reason... casted later to be the buttonTarget
    private ButtonInterface buttonTarget;  //Can call Button method on any class that implements ButtonInterface



    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<AudioSource>() == null)
        {
            Debug.Log("Error: button audiosouce missing for : " + gameObject.name);
        } else
        {
            audioSource = GetComponent<AudioSource>();
        }
        buttonTarget = target as ButtonInterface; //Casting
    }


    private void OnMouseDown()
    {
        if (PauseManager.Instance.getIsPaused()) { return; }  // Buttons can't be clicked while paused
        audioSource.Play();
        buttonTarget.Button(true, message); //Calls the Button method on whatever ButtonInterface object is specified in inspector
    }

    private void OnMouseUp()
    {
        buttonTarget.Button(false, message);
    }
}
