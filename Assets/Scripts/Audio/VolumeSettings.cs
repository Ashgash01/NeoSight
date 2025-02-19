using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class VolumeSettings : MonoBehaviour
{


    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider effectsSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey(("MasterVolume")))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetMasterVolume();
            SetEffectVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume",volume);
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        myMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    
    public void SetEffectVolume()
    {
        float volume = effectsSlider.value;
        myMixer.SetFloat("EffectVolume", volume);
        PlayerPrefs.SetFloat("EffectVolume", volume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        effectsSlider.value = PlayerPrefs.GetFloat("EffectVolume");

        SetMusicVolume();
        SetMasterVolume();
        SetEffectVolume();
    }
}
