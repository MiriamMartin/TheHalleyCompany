using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BlackoutEvent : MonoBehaviour, BlackoutInterface
{
    [Header("Lights")]
    public Material glowStrips;
    public Light emergencyLight;
    public Light leverLight;
    public List<Light> roofLights;
    private List<float> roofLightsDefault = new List<float>();


    [Header("Event")]
    public UnityEvent blackoutStart;

    // Audio Source
    private AudioSource audioSource;

    void Start()
    {
        glowStrips.DisableKeyword("_EMISSION");
        emergencyLight.intensity = 0;
        audioSource = GetComponent<AudioSource>();
        foreach (Light l in roofLights)
        {
            roofLightsDefault.Add(l.intensity);
        }
    }

    public void Run()
    {
        StartCoroutine(Blackout());
    }

    IEnumerator Blackout()
    {
        // Runs Blackout Event

        audioSource.Play(); // play powerdown audio

        // flickers roof lights
        yield return StartCoroutine(LightGroupIntensify(roofLights, roofLightsDefault, 0f, 0.1f)); //0.1
        yield return StartCoroutine(LightGroupIntensify(roofLights, roofLightsDefault, 0.9f, 0.05f)); //0.05
        yield return StartCoroutine(LightGroupIntensify(roofLights, roofLightsDefault, 0f, 0.1f));
        yield return StartCoroutine(LightGroupIntensify(roofLights, roofLightsDefault, 0.9f, 0.05f));
        yield return StartCoroutine(LightGroupIntensify(roofLights, roofLightsDefault, 0.3f, 0.8f));
        yield return StartCoroutine(LightGroupIntensify(roofLights, roofLightsDefault, 0.1f, 1f));
        yield return StartCoroutine(LightGroupIntensify(roofLights, roofLightsDefault, 0.001f, 10f));


        blackoutStart.Invoke();  // Affects CameraMovement, Sonar, and Gauge, Radio

        // Turns emergency lights on
        yield return new WaitForSeconds(1f);
        glowStrips.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(LightIntensify(emergencyLight, 15f, 3f));
        yield return StartCoroutine(LightIntensify(leverLight, 4f, 3f));
    }

    IEnumerator BlackoutEndCR()
    {
        //Janky way to remove main bright lights from lists so they don't come back on
        roofLights.RemoveAt(0);
        roofLights.RemoveAt(0);
        roofLightsDefault.RemoveAt(0);
        roofLightsDefault.RemoveAt(0);


        // When blackout over (handle pulled), start turning lights on / emergency lights off
        yield return new WaitForSeconds(1f);
        glowStrips.DisableKeyword("_EMISSION");
        yield return StartCoroutine(LightIntensify(emergencyLight, 5f, 1f));
        yield return StartCoroutine(LightIntensify(leverLight, 0f, 1f));

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(LightGroupIntensify(roofLights, roofLightsDefault, 1f, 3f));


    }

    IEnumerator LightIntensify(Light l, float endIntensity, float duration)
    {
        // Slowly turns the lights to <endIntensity> (so on/off) in <duration> time?

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

    IEnumerator LightGroupIntensify(List<Light> lights, List<float> lightsDefault, float endMult, float duration)
    {
        // Changes intensity of lights in a list to a specified endIntensityMultiplier over a specified duration in seconds
        float startMult = lights[0].intensity / lightsDefault[0];
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float mult = Mathf.Lerp(startMult, endMult, t);
        
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].intensity = lightsDefault[i] * mult;

            }
       
            yield return null;
        }

        //Making sure final values are correct (set to endMult)
        for (int i = 0; i < lights.Count; i++)
        {
            lights[i].intensity = lightsDefault[i] * endMult;

        }
    }


    public void BlackoutEnd()
    {
        StartCoroutine(BlackoutEndCR());
    }
}
