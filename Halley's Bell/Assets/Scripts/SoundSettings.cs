using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [Header("----------------- Settings -----------------")]
    [SerializeField] private TextMeshProUGUI masterSliderNum;
    [SerializeField] private TextMeshProUGUI musicSliderNum;
    [SerializeField] private TextMeshProUGUI sfxSliderNum;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public static float masterVol { get; private set; }
    public static float musicVol { get; private set; }
    public static float sfxVol { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // =======================================================

    public void SetSliderValuesOnLoad()
    {
        masterVol = PlayerPrefs.GetFloat("SavedMasterVolume", 1f);
        musicVol = PlayerPrefs.GetFloat("SavedMusicVolume", 1f);
        sfxVol = PlayerPrefs.GetFloat("SavedSFXVolume", 1f);

        masterSlider.value = masterVol;
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;
    }

    public void SetMusicVolFromSlider(float val)
    {
        musicVol = val;
        musicSliderNum.text = ((int)(val * 100)).ToString();

        PlayerPrefs.SetFloat("SavedMusicVolume", val);
        AudioHandler.Instance.UpdateMixerVolume();
    }

    public void SetMasterVolFromSlider(float val)
    {
        masterVol = val;
        masterSliderNum.text = ((int)(val * 100)).ToString();

        PlayerPrefs.SetFloat("SavedMasterVolume", val);
        AudioHandler.Instance.UpdateMixerVolume();
    }

    public void SetSFXVolFromSlider(float val)
    {
        sfxVol = val;
        sfxSliderNum.text = ((int)(val * 100)).ToString();

        PlayerPrefs.SetFloat("SavedSFXVolume", val);
        AudioHandler.Instance.UpdateMixerVolume();
    }

}
