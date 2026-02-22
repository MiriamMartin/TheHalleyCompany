using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    // Hey can we delete this one? Or do we plan to use it at some point?

    private bool on;
    private bool waiting;
    private bool blink;
    public Material mat;

    public int minTime = 10;
    public int maxTime = 15;

    public AudioSource singleAudioSource;
    public AudioSource loopAudioSource;
    public AudioClip press;
    public AudioClip alarm;

    public void Start()
    {
        mat.DisableKeyword("_EMISSION");
    }
    public void Run()
    {
        blink = false;
        StartCoroutine(waiter());

    }
    IEnumerator waiter()
    {
        waiting = true;
        loopAudioSource.Stop();
        singleAudioSource.Play();
        CancelInvoke(nameof(Blink));
        mat.DisableKeyword("_EMISSION");
        on = false;
        int waitTime = Random.Range(minTime, maxTime);
        yield return new WaitForSeconds(waitTime);
        on = true;
        InvokeRepeating("Blink", 0f, 1f);
        loopAudioSource.Play();
        waiting = false;
    }

    private void Blink()
    {
        if (blink)
        {
            mat.DisableKeyword("_EMISSION");
            blink = false;
        } else
        {
            mat.EnableKeyword("_EMISSION");
            blink = true;
        }

    }

    private void OnMouseDown()
    { 
        if (!waiting && !PauseManager.Instance.getIsPaused())
        {
            StartCoroutine(waiter());
        }

    }
}
