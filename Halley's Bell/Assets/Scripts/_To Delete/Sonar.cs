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


    // Start is called before the first frame update

    private void Start()
    {
        subNeedle.GetComponent<Renderer>().enabled = true;
        dot.GetComponent<Renderer>().enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        needle.transform.Rotate(rotationSpeed * direction * Time.deltaTime);
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

    public void BlackoutStart()
    {
        // turns sonar needle visual off during blackout

        subNeedle.GetComponent<Renderer>().enabled = false;
    }

    public void BlackoutEnd()
    {
        // turns sonar needle visual on during blackout

        subNeedle.GetComponent<Renderer>().enabled = true;
    }

}
