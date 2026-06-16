using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Texts")]
    [SerializeField] private TMP_Text masterValueText;
    [SerializeField] private TMP_Text musicValueText;
    [SerializeField] private TMP_Text sfxValueText;
    void Start()
    {
        masterSlider.value = AudioManager.instance.MasterVolume;
        musicSlider.value = AudioManager.instance.MusicVolume;
        sfxSlider.value = AudioManager.instance.SFXVolume;

        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        UpdateAllTexts();
    }

    private void OnMasterVolumeChanged(float value)
    {
        AudioManager.instance.SetMasterVolume(value);
        masterValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.instance.SetMusicVolume(value);
        musicValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.instance.SetSFXVolume(value);
        sfxValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
    }


    private void UpdateAllTexts()
    {
        masterValueText.text = $"{Mathf.RoundToInt(masterSlider.value * 100)}%";
        musicValueText.text = $"{Mathf.RoundToInt(musicSlider.value * 100)}%";
        sfxValueText.text = $"{Mathf.RoundToInt(sfxSlider.value * 100)}%";
    }
}
