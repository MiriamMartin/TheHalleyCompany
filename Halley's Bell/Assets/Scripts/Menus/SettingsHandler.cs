using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

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

    public Volume globalVolume;
    private float minBrightness = -0.47f;  // Given by 0.53, 0.53, 0.53 on the inspector numbers
    private float maxBrightness = 0.52f;   // Given by 3.08, 3.08, 3.01 on the inspector numbers

    public static float brightnessVal { get; private set; }
    
    [Header("----------------- Screen Shake Settings -----------------")]
    [SerializeField] private Toggle ScreenShakeToggle;
    public static bool ScreenShakeVal { get; private set; }



    // Start is called before the first frame update
    void Start()
    {
        UpdateBrightness();  // set brightness initially
    }

    // =========================== All ============================

    public void Update()
    {
        if (globalVolume.profile.TryGet(out ShadowsMidtonesHighlights smh))
        {
            smh.midtones.overrideState = true;
            Vector4 currentMidtones = smh.midtones.value;
            Debug.Log(currentMidtones.w);
        }
    }

    public void SetSliderValuesOnLoad()
    {
        masterVol = PlayerPrefs.GetFloat("SavedMasterVolume", 1f);
        musicVol = PlayerPrefs.GetFloat("SavedMusicVolume", 1f);
        sfxVol = PlayerPrefs.GetFloat("SavedSFXVolume", 1f);
        brightnessVal = PlayerPrefs.GetFloat("SavedBrightnessValue", 0.3f);  // default is 30, AKA 1.58 1.58 1.53
        ScreenShakeVal = PlayerPrefs.GetInt("SavedScreenShakeValue", 1) == 0 ? false : true;

        masterSlider.value = masterVol;
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;
        brightnessSlider.value = brightnessVal;
        ScreenShakeToggle.isOn = ScreenShakeVal;
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
        brightnessSliderNum.text = ((int)(brightnessVal * 100)).ToString();

        PlayerPrefs.SetFloat("SavedBrightnessValue", val);
        UpdateBrightness();
    }

    public void UpdateBrightness()
    {
        if (globalVolume.profile.TryGet(out ShadowsMidtonesHighlights smh)) 
        {
            smh.midtones.overrideState = true;
            float LightnessOffset = Mathf.Lerp(minBrightness, maxBrightness, brightnessVal);  // controls the w value AKA Lightness Offset
            float endLimit = Mathf.Lerp(0.08f, 0.1f, brightnessVal);  // controls the end limit for the shadows (goes from 0.08 to 0.1 as brightness increases)

            Vector4 currentMidtones = smh.midtones.value;
            smh.midtones.value = new Vector4(currentMidtones.x, currentMidtones.y, currentMidtones.z, LightnessOffset);
            smh.shadowsEnd.value = endLimit;
        }
    }

    // ============================ Toggle Screen Shake ==============================

    public void SetScreenShakeFromSettings(bool val)
    {
        ScreenShakeVal = val;
        int valInt = val ? 1 : 0;

        PlayerPrefs.SetInt("SavedScreenShakeValue", valInt);
        UpdateShake();
    }

    public void UpdateShake()
    {
        ScreenShakeToggle.isOn = ScreenShakeVal;
    }
}
