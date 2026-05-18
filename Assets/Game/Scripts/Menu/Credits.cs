using UnityEditor;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] private CanvasGroup creditsOverlay;
    
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private bool fadeIn = false;

    public void creditsOpen()
    {
        fadeIn = true;
        creditsOverlay.blocksRaycasts = true;
        creditsOverlay.interactable = true;
    }

    public void creditsClose()
    {
        fadeOut = true;
        creditsOverlay.blocksRaycasts = false;
        creditsOverlay.interactable = false;
    }

    void Update()
    {
        if (fadeIn)
        {
            if (creditsOverlay.alpha < 1)
            {
                creditsOverlay.alpha += Time.deltaTime;
                if (creditsOverlay.alpha >= 1)
                        fadeIn = false;
                    
            }
        }
        
        if (fadeOut)
        {
            if (creditsOverlay.alpha >= 0)
            {
                creditsOverlay.alpha -= Time.deltaTime;
                if (creditsOverlay.alpha == 0)
                    fadeOut = false;
                    
            }
        }
    }
}
