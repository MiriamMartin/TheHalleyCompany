using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSound : MonoBehaviour, HallucinationInterface
{
    public AudioSource audioSource;
    public AudioClip soundClip;


    public void Run(float volume)
    {
        audioSource.PlayOneShot(soundClip);
    }

    void UpdateVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
