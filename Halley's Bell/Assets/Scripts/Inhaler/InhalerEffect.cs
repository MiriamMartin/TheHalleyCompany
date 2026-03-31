using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class InhalerEffect : MonoBehaviour
{

    public bool DEBUGMODE = false;

    public PostProcessVolume PPVolume;
    private bool inhaled = false;
    private AudioSource audioSource;
    public GameObject playerInhaler;

    private bool curRunning = false;
    private float cooldownMin = 45f;
    private float cooldownMax = 80f;

    private float dieTime = 15f;

    private IEnumerator inhaler;

    public AudioSource Breathing;
    public AudioSource InhalerPuff;

    // Start is called before the first frame update
    void Start()
    {
        PPVolume.weight = 0;

        if (GetComponent<AudioSource>() == null)
        {
            Debug.Log("Error: Button AudioSource missing for : " + gameObject.name);
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        playerInhaler.SetActive(false);

        if (DEBUGMODE)
        {
            Run();
        }

        inhaler = Vignette(dieTime, 1);
    }

    void Run()
    {
        StartCoroutine(RunInhaler());
    }

    // Update is called once per frame
    void Update()
    {
        if (DEBUGMODE)  // when click debugmode it runs it
        {
            Run();
            DEBUGMODE = false;
        }

        if (PauseManager.Instance.getIsPaused() || InteractManager.Instance.getIsInteracting()) return;

        if (Depth.Instance.runInhaler && !curRunning)  // while inhaler events are allowed to be active
        {
            curRunning = true;
            StartCoroutine(RunInhaler());
        }
        else if (curRunning && !Depth.Instance.runInhaler)  // turns off inhaler event during blackout
        {
            killInhaler();
        }
    }

    public void killInhaler()
    {
        curRunning = false;  // event is now running
        StopCoroutine(inhaler);
        PPVolume.weight = 0;
    }

    IEnumerator RunInhaler()
    {
        Breathing.Play();
        yield return new WaitForSeconds(1);
        inhaler = Vignette(dieTime, 1);
        yield return StartCoroutine(inhaler);
    }

    IEnumerator Vignette(float duration, float endWeight)
    {
        // Increases Vignette over duration (seconds) to endWeight (0-1)

        float startWeight = PPVolume.weight;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            PPVolume.weight = Mathf.Lerp(startWeight, endWeight, t);

            if (inhaled)
            {
                Breathing.Stop();
                yield return StartCoroutine(Inhale(3.5f));
                yield return new WaitForSeconds(Random.Range(cooldownMin, cooldownMax));  // cooldown
                curRunning = false;
                yield break;
            }

            yield return null;
        }

        PPVolume.weight = endWeight;
        PauseManager.Instance.Death(); //Dies when reaches max!
    }

    IEnumerator Inhale (float duration)
    {
        // Decreases Vignette over duration (seconds) to 0, plays "animation"

        foreach (Transform child in transform) // Disable all children (make invisible) TEMP "ANIMATION"
        {
            child.gameObject.SetActive(false);
        }

        playerInhaler.SetActive(true);

        float startWeight = PPVolume.weight;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            PPVolume.weight = Mathf.Lerp(startWeight, 0, t);

            yield return null;
        }

        PPVolume.weight = 0;

        playerInhaler.SetActive(false);

        foreach (Transform child in transform) // Enable all children (make invisible) TEMP "ANIMATION"
        {
            child.gameObject.SetActive(true);
        }
    }

    private void OnMouseDown()
    {
        if (PauseManager.Instance.getIsPaused() || !curRunning) { return; }  // Buttons can't be clicked while paused
        //audioSource.Play();
        inhaled = true;
        InhalerPuff.Play();
    }

    private void OnMouseUp()
    {
        inhaled = false;
    }

}
