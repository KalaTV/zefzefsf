using UnityEngine;
using Character.Runtime;
using EnemyAttachmentSystem.Runtime;
using UnityEngine.Splines;

public class LevelManager : MonoBehaviour
{
    // Singleton pour y accéder facilement depuis n'importe où (ex: LevelManager.Instance.Save())
    public static LevelManager Instance { get; private set; }

    [Header("Références")]
    public PlayerController player;
    public PlayerAttachmentManager attachmentManager;
    public SaveSystem saveSystem;

    [Header("Paramètres")]
    public bool loadOnStart = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (loadOnStart)
        {
            LoadGameState();
        }
    }
    
    public void SaveGameState()
    {
        if (player == null || saveSystem == null) return;

        GameData data = new GameData();
        
        data.splineName = player.activeSpline.gameObject.name;
        
        data.distanceOnSpline = player.CurrentDistance;
        
        if (attachmentManager != null)
        {
            data.enemiesAttached = attachmentManager.currentAttachedCount;
        }

        saveSystem.SaveGame(data);
        Debug.Log("💾 Partie sauvegardée !");
    }
    public void LoadGameState()
    {
        GameData data = saveSystem.LoadGame();

        if (data != null)
        {
            GameObject splineObj = GameObject.Find(data.splineName);
            
            if (splineObj != null)
            {
                SplineContainer targetSpline = splineObj.GetComponent<SplineContainer>();
                
                player.SwitchSpline(targetSpline);
                
                player.currentDistance = data.distanceOnSpline;
                
                Debug.Log($"✅ Partie chargée : {data.splineName} à {data.distanceOnSpline}m");
            }
            else
            {
                Debug.LogError($"❌ Impossible de trouver la spline nommée : {data.splineName}");
            }
        }
    }
}