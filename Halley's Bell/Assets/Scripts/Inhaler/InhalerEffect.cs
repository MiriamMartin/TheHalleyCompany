using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InhalerEffect : MonoBehaviour
{

    public bool DEBUGMODE = false;

    //public PostProcessVolume PPVolume;
    private bool inhaled = false;
    public GameObject playerInhaler;

    private bool curRunning = false;
    private float cooldownMin = 45f;
    private float cooldownMax = 80f;
    private bool doneWaiting = true;

    private float dieTime = 15f;

    private IEnumerator inhaler;

    public AudioSource Breathing;
    public AudioSource InhalerPuff;
    public AudioSource Fail;

    public Light inhalerLight;

    public Volume Inhaler_Effect;        
    Vignette vignette;
    FilmGrain film_grain;

    // Start is called before the first frame update
    void Start()
    {
        Inhaler_Effect.profile.TryGet<Vignette>(out vignette);
        Inhaler_Effect.profile.TryGet<FilmGrain>(out film_grain);

        vignette.intensity.value = 0f;
        film_grain.intensity.value = 0f;
        vignette.active = true;
        film_grain.active = true;

        playerInhaler.SetActive(false);

        inhalerLight.intensity = 0;

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

        if (Depth.Instance.runInhaler && !curRunning && doneWaiting)  // while inhaler events are allowed to be active
        {
            curRunning = true;
            doneWaiting = false;
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
        vignette.intensity.value = 0f;
        film_grain.intensity.value = 0f;
        Breathing.Stop();
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

        //float startWeight = PPVolume.weight;
        float startWeight = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float w = Mathf.Lerp(startWeight, endWeight, t);
            vignette.intensity.value = w;
            film_grain.intensity.value = w;
            inhalerLight.intensity = w * 8;

            if (inhaled)
            {
                inhalerLight.intensity = 0;
                Breathing.Stop();
                yield return StartCoroutine(Inhale(7f));  // 3.5f on pitch = 2 for the sound if want quicker
                curRunning = false;
                yield return new WaitForSeconds(Random.Range(cooldownMin, cooldownMax));  // cooldown
                doneWaiting = true;
                yield break;
            }

            yield return null;
        }

        //PPVolume.weight = endWeight;
        updateDeathTip();
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

        //float startWeight = PPVolume.weight;
        //float startWeight = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            vignette.intensity.value = Mathf.Lerp(0f, 0, t);
            film_grain.intensity.value = Mathf.Lerp(0f, 0, t);

            yield return null;
        }

        //PPVolume.weight = 0;
        vignette.intensity.value = 0f;
        film_grain.intensity.value = 0f;

        playerInhaler.SetActive(false);

        foreach (Transform child in transform) // Enable all children (make invisible) TEMP "ANIMATION"
        {
            child.gameObject.SetActive(true);
        }
    }

    private void OnMouseDown()
    {
        if (PauseManager.Instance.getIsPaused()) { return; }  // Buttons can't be clicked while paused
        
        if (!curRunning)
        {
            Fail.Play();
        }
        else
        {
            inhaled = true;
            InhalerPuff.Play();
        }
    }

    private void OnMouseUp()
    {
        inhaled = false;
    }

    private void updateDeathTip()
    {
        PlayerPrefs.SetString("DeathTip", "inhaler");
    }

}
