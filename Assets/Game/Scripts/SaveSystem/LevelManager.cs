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

        // On sauvegarde toujours en mode Spline — le mode libre n'est pas persisté
        // (le joueur repart depuis la dernière spline connue au prochain lancement)

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

        // Si le joueur était en mode libre au moment de la sauvegarde,
        // on le force d'abord en mode Spline avant de switcher
        if (player.MovementMode == PlayerMovementMode.Free)
            player.ExitFreeMovement(targetSpline, data.distanceOnSpline);
        else
            player.SwitchSpline(targetSpline);

        player.currentDistance = data.distanceOnSpline;

        Debug.Log($"✅ Partie chargée : {data.splineName} à {data.distanceOnSpline}m");
    }
}
