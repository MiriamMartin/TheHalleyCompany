using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class to make specified object appear behind the player in a set position, and then dissapear after being viewed once
public class CornerOfTheEye : MonoBehaviour
{

    public CameraMovement cameraMovement;

    [Header("Setup (0 is east + 60 degrees)")]
    public int objectPosition;
    private int lookAwayDirection;
    private MeshRenderer mesh;

// Start is called before the first frame update
    void Start()
    {
        lookAwayDirection = (objectPosition - 1) % 3;
        //Debug.Log("CORNER Lookaway direction is " + lookAwayDirection);
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
        Run(); //REMOVE
    }

    void Run()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        yield return new WaitUntil(() => cameraMovement.getIsSitting());
        //Debug.Log("CORNER player is sitting");
        yield return new WaitUntil(() => cameraMovement.GetDirection() == lookAwayDirection);
        //Debug.Log("CORNER player is looking away");
        mesh.enabled = true;
        int prevDirection = lookAwayDirection;

        //maybe has to run only on turning?
        while (prevDirection == lookAwayDirection || cameraMovement.GetDirection() == lookAwayDirection)
        {
            prevDirection = cameraMovement.GetDirection();
            //Debug.Log("CORNER Waiting for movement! prev dir is" + prevDirection + "! curr direction is " + cameraMovement.GetDirection());
            yield return new WaitUntil(() => cameraMovement.GetDirection() != prevDirection);
            //Debug.Log("CORNER Rotated! prev dir is" + prevDirection + "! curr direction is " + cameraMovement.GetDirection());
            yield return null;
        }
        //Debug.Log("CORNER you saw it! (hopefully)");
        yield return new WaitForSeconds(cameraMovement.getDuration());
        mesh.enabled = false;
    }
}
