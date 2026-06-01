using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class IntroManager : MonoBehaviour
{
    [Header("Configuration UI")]
    public Image displayImage;      
    public Sprite[] introSprites; 

    private int currentIndex = 0;

    void Start()
    {
        if (introSprites.Length > 0 && displayImage != null)
        {
            displayImage.sprite = introSprites[currentIndex];
        }
        else
        {
            Debug.LogError("Il manque des images ou la référence de l'UI Image sur le script !");
        }
    }
    
    public void NextImage()
    {
        currentIndex++;
        
        if (currentIndex < introSprites.Length)
        {
            displayImage.sprite = introSprites[currentIndex];
        }
        else
        {
            OnIntroEnd();
        }
    }

    void OnIntroEnd()
    {
        Debug.Log("Fin de l'introduction !");
        SceneManager.LoadScene("LD_Blocking_Test");
    }
}