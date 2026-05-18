using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    private static readonly string BackgroundPrefs = "BackgroundPrefs";
    private static readonly string SoundEffectsPrefs = "SoundEffectsPrefs";
    
    [SerializeField] private float backGroundVolume,  soundEffectVolume;
    
    [SerializeField] private AudioSource backgroundAudio;
    [SerializeField] private AudioSource [] soundEffectAudio;
    
    void Awake()
    {
        ContinueSettings();
    }

    private void ContinueSettings()
    {
        backGroundVolume = PlayerPrefs.GetFloat(BackgroundPrefs);
        soundEffectVolume = PlayerPrefs.GetFloat(SoundEffectsPrefs);
        
        backgroundAudio.volume = backGroundVolume;

        for (int i = 0; i < soundEffectAudio.Length; i++)
        {
            soundEffectAudio[i].volume = soundEffectVolume;
        }
    }
}
