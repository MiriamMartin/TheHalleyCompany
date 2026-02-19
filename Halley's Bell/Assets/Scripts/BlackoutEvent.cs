using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BlackoutEvent : MonoBehaviour, BlackoutInterface
{
    public Material glowStrips;
    public Light roofLight;
    public Light emergencyLight;

    public UnityEvent blackoutStart;
    private AudioSource audioSource;

    void Start()
    {
        glowStrips.DisableKeyword("_EMISSION");
        emergencyLight.intensity = 0;
        audioSource = GetComponent<AudioSource>();
    }

    public void Run()
    {
        StartCoroutine(Blackout());
    }

    IEnumerator Blackout()
    {
        audioSource.Play();
        yield return StartCoroutine(LightIntensify(roofLight, 0f, 0.1f));
        yield return StartCoroutine(LightIntensify(roofLight, 0.9f, 0.05f));
        yield return StartCoroutine(LightIntensify(roofLight, 0f, 0.1f));
        yield return StartCoroutine(LightIntensify(roofLight, 0.9f, 0.05f));
        yield return StartCoroutine(LightIntensify(roofLight, 0.3f, 0.8f));
        yield return StartCoroutine(LightIntensify(roofLight, 0.1f, 1f));
        yield return StartCoroutine(LightIntensify(roofLight, 0.0f, 10f));
        blackoutStart.Invoke();


        yield return new WaitForSeconds(1f);
        glowStrips.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(LightIntensify(emergencyLight, 2f, 3f));


    }

    IEnumerator BlackoutEndCR()
    {
        yield return StartCoroutine(LightIntensify(roofLight, 0.8f, 2f));
        yield return StartCoroutine(LightIntensify(roofLight, 0.0f, 0.1f));
        yield return StartCoroutine(LightIntensify(roofLight, 1f, 0.5f));
        yield return new WaitForSeconds(2f);
        glowStrips.DisableKeyword("_EMISSION");
        yield return StartCoroutine(LightIntensify(emergencyLight, 0f, 1f));
    }

    IEnumerator LightIntensify(Light l, float endIntensity, float duration)
    {
        float startIntensity = l.intensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            l.intensity = Mathf.Lerp(startIntensity, endIntensity, t);
            yield return null;
        }

        l.intensity = endIntensity;
    }

    void Update()
    {
        
    }

    public void BlackoutEnd()
    {
        StartCoroutine(BlackoutEndCR());
    }
}
