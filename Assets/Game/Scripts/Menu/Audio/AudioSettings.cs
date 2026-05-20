using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    private static readonly string BackgroundPrefs = "BackgroundPrefs";
    private static readonly string SoundEffectsPrefs = "SoundEffectsPrefs";

    [SerializeField] private float backGroundVolume, soundEffectVolume;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    void Awake()
    {
        ContinueSettings();
    }

    private void ContinueSettings()
    {
        backGroundVolume = PlayerPrefs.GetFloat(BackgroundPrefs);
        soundEffectVolume = PlayerPrefs.GetFloat(SoundEffectsPrefs);

        // MUSIC
        float musicVolume = backGroundVolume <= 0.0001f ? -80f : Mathf.Log10(backGroundVolume) * 20;

        audioMixer.SetFloat("MusicVolume", musicVolume);

        // SFX
        float sfxVolume = soundEffectVolume <= 0.0001f ? -80f : Mathf.Log10(soundEffectVolume) * 20;

        audioMixer.SetFloat("SFXVolume", sfxVolume);
    }
}