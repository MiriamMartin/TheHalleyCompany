using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarButton : MonoBehaviour
{

    public bool clockwise = true;
    public Sonar sonar;
    public Material mat;
    private bool pressed;

    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mat.EnableKeyword("_EMISSION");
    }



    private void OnMouseDown()
    {
        if (!pressed)
        {
            audioSource.Play();
            sonar.Button(clockwise);
            mat.DisableKeyword("_EMISSION");
            pressed = true;
        }

    }
}
