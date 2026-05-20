using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string BackgroundPrefs = "BackgroundPrefs";
    private static readonly string SoundEffectsPrefs = "SoundEffectsPrefs";

    private int firstPlayInt;

    public Slider backgroundSlider, soundEffectSlider;

    private float backGroundVolume, soundEffectVolume;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    void Start()
    {
        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);

        if (firstPlayInt == 0)
        {
            backGroundVolume = .125f;
            soundEffectVolume = .5f;

            backgroundSlider.value = backGroundVolume;
            soundEffectSlider.value = soundEffectVolume;

            PlayerPrefs.SetFloat(BackgroundPrefs, backGroundVolume);
            PlayerPrefs.SetFloat(SoundEffectsPrefs, soundEffectVolume);
            PlayerPrefs.SetInt(FirstPlay, -1);
        }
        else
        {
            backGroundVolume = PlayerPrefs.GetFloat(BackgroundPrefs);
            soundEffectVolume = PlayerPrefs.GetFloat(SoundEffectsPrefs);

            backgroundSlider.value = backGroundVolume;
            soundEffectSlider.value = soundEffectVolume;
        }

        UpdateSound();
    }

    public void SaveSoundSettings()
    {
        PlayerPrefs.SetFloat(BackgroundPrefs, backgroundSlider.value);
        PlayerPrefs.SetFloat(SoundEffectsPrefs, soundEffectSlider.value);

        PlayerPrefs.Save();
    }

    void OnApplicationFocus(bool inFocus)
    {
        if (!inFocus)
        {
            SaveSoundSettings();
        }
    }

    public void UpdateSound()
    {
        // MUSIC
        float musicVolume = backgroundSlider.value <= 0.0001f ? -80f : Mathf.Log10(backgroundSlider.value) * 20;

        audioMixer.SetFloat("MusicVolume", musicVolume);

        // SFX
        float sfxVolume = soundEffectSlider.value <= 0.0001f ? -80f : Mathf.Log10(soundEffectSlider.value) * 20;

        audioMixer.SetFloat("SFXVolume", sfxVolume);
    }
}