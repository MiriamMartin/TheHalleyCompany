using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{

    public static AudioHandler Instance;

    [Header("----------------- Mixers -----------------")]

    public AudioMixerGroup masterMixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;

    [Header("----------------- Settings -----------------")]
    public GameObject Settings;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Settings.GetComponent<SoundSettings>().SetSliderValuesOnLoad();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateMixerVolume()
    {
        masterMixer.audioMixer.SetFloat("MasterVolume", Mathf.Log10(SoundSettings.masterVol) * 20f);
        musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Log10(SoundSettings.musicVol) * 20f);
        sfxMixer.audioMixer.SetFloat("SFXVolume", Mathf.Log10(SoundSettings.sfxVol) * 20f);
    }

}
