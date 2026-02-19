using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    public GameObject needle;
    public GameObject dot;
    public GameObject subNeedle;
    public float rotationSpeed = 3f;
    public Vector3 direction;
    public bool dotted = false;
    private AudioSource audioSource;
    private bool run = true;


    // Start is called before the first frame update

    private void Start()
    {
        subNeedle.GetComponent<Renderer>().enabled = true;
        dot.GetComponent<Renderer>().enabled = false;
        audioSource = GetComponent<AudioSource>();
    }


    //Taken from https://discussions.unity.com/t/rotate-spin-object-360-degrees-over-set-time-in-coroutine/622562/2
    //NOT BEING USED RN
    IEnumerator Rotate(float duration)
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;
        Debug.Log("Sonar GO!");
        subNeedle.GetComponent<Renderer>().enabled = true;

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
        subNeedle.GetComponent<Renderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            needle.transform.Rotate(rotationSpeed * direction * Time.deltaTime);
        }
    }

    //Run this method after blackout, makes ping on sonar!
    public void Ping()
    {
        if (transform.eulerAngles.y > 220 && !dotted)
        {
            StartCoroutine(PingCo());
        }
    }

    IEnumerator PingCo()
    {
        yield return new WaitUntil(() => (transform.eulerAngles.y > 220 && !dotted));
        dot.GetComponent<Renderer>().enabled = true;
        dotted = true;
        audioSource.Play();
    }

    public void Button(bool clockwise)
    {
        Debug.Log("BUTTON REMOVED");
    }

    public void BlackoutStart()
    {
        run = false;
        subNeedle.GetComponent<Renderer>().enabled = false;

    }

    public void BlackoutEnd()
    {
        run = true;
        subNeedle.GetComponent<Renderer>().enabled = true;

    }

}
