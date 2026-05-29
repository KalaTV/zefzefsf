using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class NewGame : MonoBehaviour
{
    public void ToNewGame()
    {
        string savePath = Application.persistentDataPath + "/MaSauvegarde.json";

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("🗑 Sauvegarde supprimée.");
        }

        SceneManager.LoadScene("LD_Blocking_Test");
    }
}
