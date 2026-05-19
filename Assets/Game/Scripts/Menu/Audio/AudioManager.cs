using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string BackgroundPrefs = "BackgroundPrefs";
    private static readonly string SoundEffectsPrefs = "SoundEffectsPrefs";
    private int firstPlayInt;
    
    public Slider backgroundSlider, soundEffectSlider;
    private float backGroundVolume, soundEffectVolume;
    
    [SerializeField] private AudioSource backgroundAudio;
    [SerializeField] private AudioSource [] soundEffectAudio;
    
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
            backgroundSlider.value = backGroundVolume;
            
            soundEffectVolume = PlayerPrefs.GetFloat(SoundEffectsPrefs);
            soundEffectSlider.value = soundEffectVolume;
        }
    }
        
    public void SaveSoundSettings()
    {
        PlayerPrefs.SetFloat(BackgroundPrefs, backgroundSlider.value);
        PlayerPrefs.SetFloat(SoundEffectsPrefs, soundEffectSlider.value);
    }

    void OnApplicationFocus(bool inFocus)
    {
        if (!inFocus)
        {
            SaveSoundSettings();
        }
    }
    
    public void UpdateSound ()
    {
        backgroundAudio.volume = backgroundSlider.value;

        for (int i = 0; i < soundEffectAudio.Length; i++)
        {
            soundEffectAudio[i].volume = soundEffectSlider.value;
        }
    }
}
