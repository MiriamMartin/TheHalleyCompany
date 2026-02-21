using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{

    public static AudioHandler Instance;
    private bool gameScene = false;  // if not game scene, ignores depth script not existing

    [Header("----------------- Mixers -----------------")]

    public AudioMixerGroup masterMixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;

    [Header("----------------- Settings -----------------")]
    public GameObject SettingsMenu;

    [Header("----------------- Movement -----------------")]
    public AudioSource walkAudio;
    public AudioSource rotateAudio;

    [Header("----------------- Hallucinations -----------------")]
    public AudioSource HallucinationSRC;  // Might want to do list in various areas, randomly pick one
    public List<AudioClip> HallucinationClips = new List<AudioClip>();
    public List<float> HallucinationClipsVolume = new List<float>();  // temp vol normalization between clips
    public float hallucinateInterval = 10f;
    private bool hallucinateRunning = false;
    public AudioSource radioSRC;  // also temp, no time

    [Header("----------------- Ambient -----------------")]
    public AudioSource groanSRC;  // Might want to do list in various areas, randomly pick one
    public List<AudioClip> groanClips = new List<AudioClip>();
    private bool groanRunning = false;
    

    void Start()
    {
        Instance = this;
        SettingsMenu.GetComponent<SettingsHandler>().SetSliderValuesOnLoad();

        if (Depth.Instance != null)
        {
            gameScene = true;
        }
    }

    void Update()
    {
        if (!gameScene) { return; }

        if (Depth.Instance.canHallucinate && !hallucinateRunning)
        {
            hallucinateRunning = true;
            StartCoroutine(Hallucinate());
        }
        if (!groanRunning)
        {
            groanRunning = true;
            StartCoroutine(AmbientGroans()); 
        } 
    }

    // ============================ Mixer / Settings ===============================
    public void UpdateMixerVolume()
    {
        masterMixer.audioMixer.SetFloat("MasterVolume", Mathf.Log10(SettingsHandler.masterVol) * 20f);
        musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Log10(SettingsHandler.musicVol) * 20f);
        sfxMixer.audioMixer.SetFloat("SFXVolume", Mathf.Log10(SettingsHandler.sfxVol) * 20f);
    }

    // =============================== Movement ===================================
    public void PlayMovement(int val)
    {
        // val = 1 means rotate, val = 2 means walk

        // iirc don't happen at same time so fine, but if do can separate their sources

        if (val == 1)
        {
            rotateAudio.Play();
        }
        else if (val == 2)
        {
            walkAudio.Play();
        }
    }

    // ============================== Hallucinations ===================================
    public void PlayRandomHallucination()
    {
        // Plays random hallucination sound

        // random pan
        HallucinationSRC.panStereo = Random.Range(-1.0f, 1.0f);

        // random clip
        int randIndex = Random.Range(0, HallucinationClips.Count);
        AudioClip toPlay = HallucinationClips[randIndex];
        HallucinationSRC.volume = HallucinationClipsVolume[randIndex];
        HallucinationSRC.clip = toPlay;
        HallucinationSRC.Play();

    }

    public IEnumerator Hallucinate()
    {
        yield return new WaitForSeconds(hallucinateInterval);
        yield return new WaitUntil(() => !radioSRC.isPlaying);  // don't overlap w/ radio
        PlayRandomHallucination();
        yield return new WaitUntil(() => !HallucinationSRC.isPlaying);
        hallucinateRunning = false;
    }

    public void SetHallucinateInterval(float val)
    {
        hallucinateInterval = val;
    }

    // ============================== Groans ===================================
    // can later be combined w/ hallucinations in reusable functions

    public IEnumerator AmbientGroans()
    {
        yield return new WaitForSeconds(Random.Range(10f,45f));
        PlayRandomGroan();
        yield return new WaitUntil(() => !groanSRC.isPlaying);
        groanRunning = false;
    }

    public void PlayRandomGroan()
    {
        // Play random clip
        AudioClip toPlay = groanClips[Random.Range(0, groanClips.Count)];
        groanSRC.clip = toPlay;
        groanSRC.Play();
    }
}
