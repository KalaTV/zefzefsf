using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class SettingsOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup settingsOverlay;

    public void Open()
    {
        settingsOverlay.alpha = 1;
        settingsOverlay.blocksRaycasts = true;
        settingsOverlay.interactable = true;
    }

    public void Close()
    {
        settingsOverlay.alpha = 0;
        settingsOverlay.blocksRaycasts = false;
        settingsOverlay.interactable = false;
    }
}
