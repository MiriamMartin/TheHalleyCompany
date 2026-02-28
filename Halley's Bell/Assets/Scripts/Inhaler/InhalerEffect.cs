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
    }

    void Run()
    {
        StartCoroutine(RunInhaler());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RunInhaler()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(Vignette(10, 1));
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
                yield return StartCoroutine(Inhale(1));
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
        if (PauseManager.Instance.getIsPaused()) { return; }  // Buttons can't be clicked while paused
        //audioSource.Play();
        inhaled = true;
    }

    private void OnMouseUp()
    {
        inhaled = false;
    }

}
