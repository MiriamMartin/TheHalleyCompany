using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    public GameObject needle;
    public GameObject dot;
    public float rotationSpeed = 3f;
    public Vector3 direction;
    public bool dotted = false;
    private AudioSource audioSource;


    // Start is called before the first frame update

    private void Start()
    {
        needle.GetComponent<Renderer>().enabled = true;
        dot.GetComponent<Renderer>().enabled = false;
        audioSource = GetComponent<AudioSource>();
    }


    //Taken from https://discussions.unity.com/t/rotate-spin-object-360-degrees-over-set-time-in-coroutine/622562/2
    IEnumerator Rotate(float duration)
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;
        Debug.Log("Sonar GO!");
        needle.GetComponent<Renderer>().enabled = true;

        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
            if (yRotation > 220 && !dotted)
            {
                dot.GetComponent<Renderer>().enabled = true;
                dotted = true;
                audioSource.Play();
            }
        }
        needle.GetComponent<Renderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        needle.transform.Rotate(rotationSpeed * direction * Time.deltaTime);
    }

    public void Button(bool clockwise)
    {
        Debug.Log("BUTTON REMOVED");
    }

}
