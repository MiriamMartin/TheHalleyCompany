using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour
{

    public AudioSource screwAudio;
    public AudioSource popAudio;
    public GameObject wrench;
    private Wrench wrenchScript;
    private int cranks = 0;

    void Start()
    {
        wrenchScript = wrench.GetComponent<Wrench>();
    }

    private void OnMouseDown()
    {
        // when clicked, put wrench in position beside it. If wrench already beside it, put back in hand.

        if (wrenchScript.getCurrentScrew() == this.name)  // if clicking on screw that currently on, get off.
        {
            resetWrench();
            wrenchScript.setOnScrew(false);
            wrenchScript.setCurrentScrew(null);
        }
        else  // if clicking on screw NOT currently on attach to / switch to that screw.
        {
            positionWrench();
            wrenchScript.setOnScrew(true);
            wrenchScript.setCurrentScrew(gameObject);
        }

    }

    public void positionWrench()
    {
        // Moves the wrench into position (to the right of the screws)
        Transform oldParent = wrench.transform.parent;
        wrench.transform.parent = transform;

        wrench.transform.localPosition = new Vector3(0f, 0f, 0.03f);
        wrench.transform.localRotation = Quaternion.identity;

        wrench.transform.parent = oldParent;
    }

    public void increaseCranks()
    {
        // increases the number of cranks on this screw
        
        cranks++;
        setScrewRotation();

        if (cranks >= 3)
        {
            StartCoroutine(pauseUndone());
        }
    }

    public IEnumerator pauseUndone()
    {
        // gives brief time for screw to rotate last time and play sound before popping off

        yield return new WaitForSeconds(0.5f);
        undone();
    }
    public void undone()
    {
        // when screw undone, turns it off

        popAudio.Play();
        resetWrench();
        gameObject.SetActive(false);
    }

    public void setScrewRotation()
    {
        // rotates the screw with the wrench (once per crank, gives player vis indicator of progress)

        //transform.localRotation = Quaternion.Euler(transform.localRotation.z + (200f * cranks), -90f, -90f);
        transform.localRotation = Quaternion.Euler(0f, 180f, transform.localRotation.z + (200f * cranks));
        screwAudio.Play();

        // moves screw out a bit || MIGHT NEED TO CHANGE FROM Z TO X
        transform.position += Vector3.back * 0.02f;
    }

    public void resetWrench()
    {
        // idk man this works but just re-holding it doesn't

        wrenchScript.DropWrench();
        wrenchScript.HoldWrench();
    }
}
