using UnityEngine;

public class TutoPop : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private bool hasBeenShown = false;

    public void ShowPopup()
    {
        if (hasBeenShown)
            return;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            panel.SetActive(true);
            hasBeenShown = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ShowPopup();
    }
}