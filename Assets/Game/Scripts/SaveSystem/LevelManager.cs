using UnityEngine;
using Character.Runtime;
using EnemyAttachmentSystem.Runtime;
using UnityEngine.Splines;

public class LevelManager : MonoBehaviour
{
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
            LoadGameState();
    }

    public void SaveGameState()
    {
        if (player == null || saveSystem == null) return;

        GameData data = new GameData();

        data.splineName       = player.activeSpline != null ? player.activeSpline.gameObject.name : "";
        data.distanceOnSpline = player.CurrentDistance;

        if (attachmentManager != null)
            data.enemiesAttached = attachmentManager.currentAttachedCount;

        saveSystem.SaveGame(data);
        Debug.Log("💾 Partie sauvegardée !");
    }

    public void LoadGameState()
    {
        GameData data = saveSystem.LoadGame();
        if (data == null) return;

        if (string.IsNullOrEmpty(data.splineName)) return;

        GameObject splineObj = GameObject.Find(data.splineName);
        if (splineObj == null)
        {
            Debug.LogWarning($"[LevelManager] Spline '{data.splineName}' introuvable dans la scène.");
            return;
        }

        SplineContainer targetSpline = splineObj.GetComponent<SplineContainer>();
        if (targetSpline == null)
        {
            Debug.LogWarning($"[LevelManager] Pas de SplineContainer sur '{data.splineName}'.");
            return;
        } 
        
        if (player.MovementMode == PlayerMovementMode.Free)
            player.ExitFreeMovement(targetSpline, data.distanceOnSpline);
        else
            player.SwitchSpline(targetSpline);

        player.currentDistance = data.distanceOnSpline;

        Debug.Log($"✅ Partie chargée : {data.splineName} à {data.distanceOnSpline}m");
    }
}
