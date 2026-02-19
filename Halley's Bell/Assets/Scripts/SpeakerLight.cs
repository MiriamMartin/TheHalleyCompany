using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerLight : MonoBehaviour
{
    // Start is called before the first frame update
    public Material mat;
    public AudioLoudnessDetection ald;
    public Radio radio;
    public AudioSource audioSource;

    public float intensity = 1f;
    private float currIntensity;
    [SerializeField] float lightBrightness;

    void Start()
    {
        currIntensity = intensity;
    }

    // Update is called once per frame
    void Update()
    {
        float colourMult = ald.GetLoudnessFromAudio(audioSource.timeSamples, audioSource.clip) * currIntensity;
        mat.SetColor("_EmissionColor", Color.yellow * colourMult * currIntensity);
        lightBrightness = ald.GetLoudnessFromAudio(audioSource.timeSamples, audioSource.clip) * currIntensity;
    }

    public void setLightAudioSource(AudioSource audioSource)
    {
        currIntensity = intensity;
        this.audioSource = audioSource;
    }

    public void disableLight()
    {
        currIntensity = 0f;
    }
}
