using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    [Header("----------------- Sound Settings -----------------")]
    [SerializeField] private TextMeshProUGUI masterSliderNum;
    [SerializeField] private TextMeshProUGUI musicSliderNum;
    [SerializeField] private TextMeshProUGUI sfxSliderNum;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public static float masterVol { get; private set; }
    public static float musicVol { get; private set; }
    public static float sfxVol { get; private set; }

    [Header("----------------- Brightness Settings -----------------")]
    [SerializeField] private TextMeshProUGUI brightnessSliderNum;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Image brightnessImg;

    public static float brightnessVal { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        UpdateBrightness();  // set brightness initially
    }

    // =========================== All ============================

    public void SetSliderValuesOnLoad()
    {
        masterVol = PlayerPrefs.GetFloat("SavedMasterVolume", 1f);
        musicVol = PlayerPrefs.GetFloat("SavedMusicVolume", 1f);
        sfxVol = PlayerPrefs.GetFloat("SavedSFXVolume", 1f);
        brightnessVal = PlayerPrefs.GetFloat("SavedBrightnessValue", 0.001f);

        masterSlider.value = masterVol;
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;
        brightnessSlider.value = brightnessVal;
    }

    // ========================== Audio =============================

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

    //  ============================ Brightness ==============================
    public void SetBrightnessFromSlider(float val)
    {
        brightnessVal = val;
        brightnessSliderNum.text = ((int)(val * 1000)).ToString();  // slider goes to 0.1, 0.1 x 1000 = 100 for display purposes

        PlayerPrefs.SetFloat("SavedBrightnessValue", val);
        UpdateBrightness();
    }

    public void UpdateBrightness()
    {
        Color brightnessCol = brightnessImg.color;
        brightnessCol.a = brightnessVal;
        brightnessImg.color = brightnessCol;
    }
}
