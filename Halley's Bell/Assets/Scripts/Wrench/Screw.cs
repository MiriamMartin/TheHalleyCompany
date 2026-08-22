using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour
{

    public AudioSource loosenAudio;
    public AudioSource popAudio;
    public AudioSource tightenAudio;
    public GameObject wrench;
    private Wrench wrenchScript;
    private int cranks = 0;

    public bool BH_nut = false;

    void Start()
    {
        wrenchScript = wrench.GetComponent<Wrench>();

        if (BH_nut && Checkpoints.Instance.getCheckpoint() >= 6)
        {
            gameObject.SetActive(false);
        }
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
            if (wrenchScript.isHoldingWrench)
            {
                positionWrench();
                wrenchScript.setOnScrew(true);
                wrenchScript.setCurrentScrew(gameObject);
            }

        }

    }

    public void positionWrench()
    {
        // Moves the wrench into position (to the right of the screws)
        Transform oldParent = wrench.transform.parent;
        wrench.transform.parent = transform;

        wrench.transform.localPosition = new Vector3(0f, 0f, -0.02f);
        //wrench.transform.localRotation = Quaternion.identity;
        wrench.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        wrench.transform.parent = oldParent;
    }

    public void loosen()
    {
        // increases the number of cranks on this screw
        
        cranks++;
        loosenAudio.Play();
        setScrewRotation(-1);

        if (cranks >= 3)
        {
            wrenchScript.setClickAgain(true);
            StartCoroutine(pauseUndone());
        }
    }

    public void tighten()
    {
        cranks--;
        tightenAudio.Play();

        if (cranks < 0)
        {
            wrenchScript.setClickAgain(true);
            cranks = 0;
        }
        else
        {
            setScrewRotation(1);
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

    public void setScrewRotation(int dir)
    {
        // rotates the screw with the wrench (once per crank, gives player vis indicator of progress)

        //transform.localRotation = Quaternion.Euler(transform.localRotation.z + (200f * cranks), -90f, -90f);
        transform.localRotation = Quaternion.Euler(0f, 180f, transform.localRotation.z + (dir * 200f * cranks));

        // moves screw out a bit || MIGHT NEED TO CHANGE FROM Z TO X
        if (!BH_nut) { transform.position += Vector3.back * 0.02f * dir; }
        else { transform.position += Vector3.left * 0.02f * dir; } 
    }

    public void resetWrench()
    {
        // idk man this works but just re-holding it doesn't

        wrenchScript.DropWrench();
        wrenchScript.HoldWrench();
    }
}
