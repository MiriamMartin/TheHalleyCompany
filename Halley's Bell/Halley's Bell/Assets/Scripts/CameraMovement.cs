using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float angleAmount = 180f;
    public float duration = 0.5f;
    private bool isRotating = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(Rotate(1));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Rotate(-1));
            }
        }

    }

    
    IEnumerator Rotate(int dir)
    {
        isRotating = true;
        Debug.Log("Rotating " + dir);
        float elapsed = 0f;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.AngleAxis(180f * dir, Vector3.up);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
    }
    

}
