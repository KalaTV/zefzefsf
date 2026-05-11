using System.Collections;
using UnityEngine;
using Character.Runtime;

[RequireComponent(typeof(Collider))]
public class WaterHandSpawner : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Prefab de la main (doit avoir WaterHandController)")]
    public GameObject handPrefab;

    [Header("Spawn")]
    [Tooltip("Intervalle en secondes entre chaque spawn de main")]
    public float spawnInterval = 3f;

    [Tooltip("Profondeur sous la surface à laquelle la main apparaît (valeur positive = vers le bas)")]
    public float spawnDepth = 2f;

    [Tooltip("Rayon aléatoire autour du joueur pour le point de spawn (évite spawn exactement sous lui)")]
    [Range(0f, 3f)] public float spawnRadiusAroundPlayer = 1.5f;

    [Header("Limites")]
    [Tooltip("Nombre max de mains actives en même temps (1 = une seule à la fois)")]
    public int maxHandsAtOnce = 1;

    [Tooltip("Détruire les mains actives quand le joueur quitte la zone")]
    public bool destroyHandsOnExit = true;

    // ─────────────────────────────────────────
    // État interne
    // ─────────────────────────────────────────
    private PlayerController player;
    private bool playerInZone = false;
    private int activeHandCount = 0;
    private Coroutine spawnRoutine;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    // ─────────────────────────────────────────
    // Détection zone
    // ─────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.GetComponent<PlayerController>();
        if (player == null) return;

        playerInZone = true;
        spawnRoutine = StartCoroutine(SpawnLoop());

        Debug.Log("[WaterHandSpawner] Joueur entré dans la zone d'eau.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInZone = false;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        if (destroyHandsOnExit)
            DestroyAllHands();

        player = null;

        Debug.Log("[WaterHandSpawner] Joueur sorti de la zone d'eau.");
    }

    // ─────────────────────────────────────────
    // Boucle de spawn
    // ─────────────────────────────────────────
    private IEnumerator SpawnLoop()
    {
        // Petit délai initial pour laisser le joueur entrer dans la zone
        yield return new WaitForSeconds(1f);

        while (playerInZone)
        {
            if (activeHandCount < maxHandsAtOnce && player != null)
                SpawnHand();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnHand()
    {
        if (handPrefab == null)
        {
            Debug.LogWarning("[WaterHandSpawner] Aucun prefab de main assigné !", this);
            return;
        }

        // Point de spawn : position du joueur + offset aléatoire horizontal + profondeur vers le bas
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadiusAroundPlayer;
        Vector3 spawnPos = new Vector3(
            player.transform.position.x + randomCircle.x,
            player.transform.position.y - spawnDepth,
            player.transform.position.z + randomCircle.y
        );

        // Clamp dans les limites de la zone
        Bounds bounds = GetComponent<Collider>().bounds;
        spawnPos.x = Mathf.Clamp(spawnPos.x, bounds.min.x, bounds.max.x);
        spawnPos.z = Mathf.Clamp(spawnPos.z, bounds.min.z, bounds.max.z);

        GameObject handObj = Instantiate(handPrefab, spawnPos, Quaternion.identity);
        WaterHandController hand = handObj.GetComponent<WaterHandController>();

        if (hand == null)
        {
            Debug.LogError("[WaterHandSpawner] Le prefab n'a pas de WaterHandController !", this);
            Destroy(handObj);
            return;
        }

        hand.Init(player);
        activeHandCount++;

        // Décrémenter le compteur quand la main est détruite
        hand.OnHandDestroyed += () => activeHandCount = Mathf.Max(0, activeHandCount - 1);

        Debug.Log($"[WaterHandSpawner] Main spawée à {spawnPos}");
    }

    // ─────────────────────────────────────────
    // Nettoyage
    // ─────────────────────────────────────────
    private void DestroyAllHands()
    {
        // Trouver toutes les mains actives dans la scène et les détruire
        foreach (var hand in FindObjectsByType<WaterHandController>(FindObjectsSortMode.None))
            Destroy(hand.gameObject);

        activeHandCount = 0;
    }

    private void OnDestroy()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Visualiser la zone d'eau en bleu transparent
        Gizmos.color = new Color(0.1f, 0.4f, 1f, 0.12f);
        var col = GetComponent<Collider>();
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = new Color(0.1f, 0.4f, 1f, 0.5f);
            Gizmos.DrawWireCube(box.center, box.size);
        }

        // Visualiser la profondeur de spawn
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.6f);
        Vector3 center = col != null ? col.bounds.center : transform.position;
        Gizmos.DrawLine(center, center + Vector3.down * spawnDepth);
        Gizmos.DrawWireSphere(center + Vector3.down * spawnDepth, 0.2f);
    }
#endif
}