using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerLight : MonoBehaviour
{
    // Start is called before the first frame update
    public Material mat;
    public AudioLoudnessDetection ald;
    public AudioSource audioSource;
    public float intensity = 5f;
    [SerializeField] float lightBrightness;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mat.SetColor("_EmissionColor", Color.yellow * ald.GetLoudnessFromAudio(audioSource.timeSamples, audioSource.clip) * intensity);
        lightBrightness = ald.GetLoudnessFromAudio(audioSource.timeSamples, audioSource.clip) * intensity;
    }
}
