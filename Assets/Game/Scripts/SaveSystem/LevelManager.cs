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

    [Header("Free Mode Settings")]
    public float freeMoveSpeed = 5f;
    public float freeRotSpeed  = 8f;
    public bool  freeApplyGravity = true;

    [Header("Paramètres")]
    public bool loadOnStart = true;

    public void NewGame()
    {
        saveSystem.DeleteSave();
        UnityEngine.SceneManagement.SceneManager.LoadScene("NomDeTaScene");
    }
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
        data.isFreeMode      = player.MovementMode == PlayerMovementMode.Free;
        data.enemiesAttached = attachmentManager != null ? attachmentManager.currentAttachedCount : 0;

        if (data.isFreeMode)
        {
            data.posX = player.transform.position.x;
            data.posY = player.transform.position.y;
            data.posZ = player.transform.position.z;
        }
        else
        {
            data.splineName       = player.activeSpline != null ? player.activeSpline.gameObject.name : "";
            data.distanceOnSpline = player.CurrentDistance;
        }

        saveSystem.SaveGame(data);
        Debug.Log("💾 Partie sauvegardée !");
    }

    public void LoadGameState()
    {
        GameData data = saveSystem.LoadGame();
        if (data == null) return;

        if (attachmentManager != null)
            attachmentManager.currentAttachedCount = data.enemiesAttached;

        if (data.isFreeMode)
        {
            // Si le player est sur spline, on entre en free mode d'abord
            if (player.MovementMode != PlayerMovementMode.Free)
                player.EnterFreeMovement(freeMoveSpeed, freeRotSpeed, freeApplyGravity);

            // Déplace le player à la position sauvegardée
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = new Vector3(data.posX, data.posY, data.posZ);
            player.GetComponent<CharacterController>().enabled = true;

            Debug.Log($"✅ Partie chargée en Free Mode : {player.transform.position}");
        }
        else
        {
            if (string.IsNullOrEmpty(data.splineName))
            {
                Debug.LogWarning("[LevelManager] Aucune spline sauvegardée.");
                return;
            }

            GameObject splineObj = GameObject.Find(data.splineName);
            if (splineObj == null)
            {
                Debug.LogWarning($"[LevelManager] Spline '{data.splineName}' introuvable.");
                return;
            }

            SplineContainer targetSpline = splineObj.GetComponent<SplineContainer>();
            if (targetSpline == null)
            {
                Debug.LogWarning($"[LevelManager] Pas de SplineContainer sur '{data.splineName}'.");
                return;
            }

            // ExitFreeMovement gère lui-même SwitchSpline + currentDistance
            if (player.MovementMode == PlayerMovementMode.Free)
                player.ExitFreeMovement(targetSpline, data.distanceOnSpline);
            else
            {
                player.SwitchSpline(targetSpline);
                player.currentDistance = data.distanceOnSpline;
            }

            Debug.Log($"✅ Partie chargée : {data.splineName} à {data.distanceOnSpline}m");
        }
    }
}