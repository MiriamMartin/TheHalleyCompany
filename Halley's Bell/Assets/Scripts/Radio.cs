using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour, ButtonInterface
{
    const float minFreq = 88f;
    const float maxFreq = 108;
    public float range = 5f;

    //Button stuff
    private bool mouseDown;
    private string message;

    public AudioSource fuzz;
    public AudioSource a1;
    public AudioSource a2;
    public AudioSource a3;

    private bool on = true;

    [Range(minFreq, maxFreq)]
    public float freq;
    public float increaseRate = 10;

    public Transform needle;
    private float needleZ;
    public float distanceMod = 1;

    // Start is called before the first frame update
    void Start()
    {
        Run();
        needleZ = needle.position.z;
        freq = minFreq;
    }

    void Run()
    {
        a1.volume = 0;
        a2.volume = 0;
        a3.volume = 0;
        fuzz.volume = 1;
        a1.Play();
        a2.Play();
        a3.Play();
        fuzz.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            //Updating state of radioPoints (mostly audio for now)
            radioPoint(a1, 92);
            //radioPoint(a2, 92);
            //radioPoint(a3, 100);

            //Increasing or decreasing the freq over time depending on which button pressed
            if (mouseDown && message == "increase")
            {
                freq += increaseRate * Time.deltaTime;
                freq = Mathf.Clamp(freq, minFreq, maxFreq); //Contraining the range
            }
            else if (mouseDown && message == "decrease")
            {
                freq -= increaseRate * Time.deltaTime;
                freq = Mathf.Clamp(freq, minFreq, maxFreq); //Contraining the range
            }
            else if (message == "power" && mouseDown)
            {
                message = "";
                on = false;
            }

            needle.position = new Vector3(needle.position.x, needle.position.y, needleZ + ((freq - minFreq) * distanceMod));
        }
        else if (mouseDown)
        {
            a1.volume = 0;
            a2.volume = 0;
            a3.volume = 0;
            fuzz.volume = 0;
            if (message == "power")
            {
                Debug.Log("poweron!");
                fuzz.volume = 1;
                message = "";
                on = true;
            }
        }


    }

    //returns a float from 0-1 proportional to how close it is to the target frequency. (1 is on the freq, distance +/- range is 0)
    float getCloseness(float target)
    {
        if ((freq <= target + range) && (freq >= target - range))
        {
            return (1 - (Mathf.Abs(target - freq) / range));
        }
        return 0;
    }

    void radioPoint(AudioSource audioSource, float target)
    {
        float closeness = getCloseness(target);
        audioSource.volume = closeness;
        if (closeness > 0)
        {
            fuzz.volume = (1 - closeness);
        }
    }

    public void Button(bool mouseDown, string message)
    {
        this.mouseDown = mouseDown;
        this.message = message;
    }
}
