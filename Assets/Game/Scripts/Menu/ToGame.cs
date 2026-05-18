using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToGame : MonoBehaviour
{
    public void ToGameStart()
    {
        SceneManager.LoadScene("LD_Blocking_Test");
    }
}
