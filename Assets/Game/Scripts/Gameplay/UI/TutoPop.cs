using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TutoPop : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    
    public void ShowPopup()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            panel.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ShowPopup();
    }
}
