using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauge : MonoBehaviour, ButtonInterface
{

    public float speed = 5f;
    public float buttonPower = 5f;
    public Vector3 direction = new Vector3();
    public GameObject bulb;
    public bool blinking = false;
    public bool blink;
    private Material mat;
    private bool run = false;

    public AudioSource audioSource;


    private void Start()
    {
        Renderer rend = bulb.GetComponent<Renderer>();
        mat = rend.material;
        mat.DisableKeyword("_EMISSION");
        blink = false;
        blinking = false;
    }

    public void Run()
    {
        run = true;
    }

    private void Update()
    {
        if (run)
        {
            transform.Rotate(speed * direction * Time.deltaTime);
            if ((transform.eulerAngles.z > 50) && !blinking && (transform.eulerAngles.z < 90))
            {
                Debug.Log("Guage Blinking");
                audioSource.Play();
                InvokeRepeating("Blink", 0f, 0.2f);
                blinking = true;
            } else if ((transform.eulerAngles.z <= 50) && blinking)
            {
                Debug.Log("Guage Stopped Blinking");
                CancelInvoke(nameof(Blink));
                audioSource.Stop();
                blinking = false;
                mat.DisableKeyword("_EMISSION");
            }
        }
 
    }

    private void Blink()
    {
        if (blink)
        {
            mat.DisableKeyword("_EMISSION");
            blink = false;
        }
        else
        {
            mat.EnableKeyword("_EMISSION");
            blink = true;
        }

    }

    public void Button(bool mouseDown, string message)
    {
        if (mouseDown && !PauseManager.Instance.getIsPaused())
        {
            if (message == "clockwise")
            {
                Debug.Log("clockwise");
                transform.Rotate(buttonPower * direction);
            }
            else
            {
                Debug.Log("counterclockwise");
                transform.Rotate(buttonPower * -direction);
            }
        }
 
    }
}
