using UnityEngine;
using System.IO; 

public class SaveSystem : MonoBehaviour
{
    private string saveFilePath;

    void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/MaSauvegarde.json";
    }
    public void SaveGame(GameData dataToSave)
    {
        string json = JsonUtility.ToJson(dataToSave, true);
        
        File.WriteAllText(saveFilePath, json);
        
        Debug.Log("Jeu sauvegardé avec succès dans : " + saveFilePath);
    }
    public void DeleteSave()
    {

        // Si tu utilises un fichier JSON/binaire
        string path = Application.persistentDataPath + "/save.dat"; // adapte le nom
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
    }
    public GameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            
            
            GameData loadedData = JsonUtility.FromJson<GameData>(json);
            
            Debug.Log("Jeu chargé avec succès !");
            return loadedData;
        }
        else
        {
            Debug.LogWarning("️Aucun fichier de sauvegarde trouvé.");
            return null; 
        }
    }
}