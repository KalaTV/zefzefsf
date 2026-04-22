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
        
        Debug.Log("✅ Jeu sauvegardé avec succès dans : " + saveFilePath);
    }
    
    public GameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            
            
            GameData loadedData = JsonUtility.FromJson<GameData>(json);
            
            Debug.Log("✅ Jeu chargé avec succès !");
            return loadedData;
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun fichier de sauvegarde trouvé.");
            return null; 
        }
    }
}