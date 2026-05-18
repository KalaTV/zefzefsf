using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    public void OpenMenu()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
    
    public void CloseMenu()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}
