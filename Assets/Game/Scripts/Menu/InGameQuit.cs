using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameQuit : MonoBehaviour
{
     public void gameQuit()
        {
            SceneManager.LoadScene("Start_Menu");
        }
}
