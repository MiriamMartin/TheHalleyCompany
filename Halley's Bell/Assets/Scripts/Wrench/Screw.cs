using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour
{

    public AudioSource screwAudio;
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

        Vector3 newPos = new Vector3(transform.localPosition.x + 0.1f, transform.localPosition.y, transform.localPosition.z + 4.3f);
        wrench.transform.localPosition = newPos;
        wrench.transform.localRotation = Quaternion.identity;
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

        resetWrench();
        gameObject.SetActive(false);
    }

    public void setScrewRotation()
    {
        // rotates the screw with the wrench (once per crank, gives player vis indicator of progress)

        transform.localRotation = Quaternion.Euler(transform.localRotation.z + (200f * cranks), -90f, -90f);
        screwAudio.Play();
    }

    public void resetWrench()
    {
        // idk man this works but just re-holding it doesn't

        wrenchScript.DropWrench();
        wrenchScript.HoldWrench();
    }
}
