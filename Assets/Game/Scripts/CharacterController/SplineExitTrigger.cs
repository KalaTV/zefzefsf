using UnityEngine;
using Character.Runtime;
using UnityEngine.Splines;

[RequireComponent(typeof(Collider))]
public class SplineExitTrigger : MonoBehaviour
{
    [Header("Spline de destination")]
    [Tooltip("La SplineContainer sur laquelle le joueur sera raccroché à la sortie")]
    public SplineContainer targetSpline;

    [Header("Options de transition")]
    [Tooltip("Si activé, le joueur sera placé au point le plus proche de la spline")]
    public bool snapToNearestPoint = true;

    [Tooltip("Direction de départ sur la spline (1 = avant, -1 = arrière)")]
    [Range(-1f, 1f)]
    public float startDirection = 1f;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null) return;
        
        if (player.MovementMode == PlayerMovementMode.Spline &&
            player.activeSpline == targetSpline) return;
        
        if (player.MovementMode != PlayerMovementMode.Free) return;

        if (targetSpline == null)
        {
            Debug.LogWarning($"[SplineExitTrigger] {gameObject.name} : aucune spline assignée !", this);
            return;
        }
        
        float initialDistance = 0f;
        if (snapToNearestPoint)
        {
            initialDistance = GetNearestDistanceOnSpline(player.transform.position);
        }
        
        player.ExitFreeMovement(targetSpline, initialDistance);

        Debug.Log($"[SplineExitTrigger] Joueur raccroché sur '{targetSpline.gameObject.name}' à {initialDistance:F2}m");
    }
    
    private float GetNearestDistanceOnSpline(Vector3 worldPos)
    {
        if (targetSpline == null) return 0f;

        float splineLength = targetSpline.CalculateLength();
        int steps = 64;
        float bestDist = 0f;
        float bestSqrMag = float.MaxValue;

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float dist = t * splineLength;
            // Évaluer la position sur la spline à ce t normalisé
            Vector3 point = targetSpline.EvaluatePosition(t);
            float sqrMag = (point - worldPos).sqrMagnitude;
            if (sqrMag < bestSqrMag)
            {
                bestSqrMag = sqrMag;
                bestDist = dist;
            }
        }

        return bestDist;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.15f);
        var col = GetComponent<Collider>();
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.6f);
            Gizmos.DrawWireCube(box.center, box.size);
        }
        
        if (targetSpline != null)
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.8f);
            Gizmos.DrawLine(transform.position, targetSpline.transform.position);
            Gizmos.DrawSphere(targetSpline.transform.position, 0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 1.5f,
            $"→ {(targetSpline != null ? targetSpline.gameObject.name : "AUCUNE SPLINE")}",
            UnityEditor.EditorStyles.boldLabel
        );
    }
#endif
}
