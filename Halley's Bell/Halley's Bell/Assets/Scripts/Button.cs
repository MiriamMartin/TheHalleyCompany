using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    public bool clockwise = true;
    public Gauge gauge;
    private AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnMouseDown()
    {
        audioSource.Play();
        gauge.Button(clockwise);
    }
}
