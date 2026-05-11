using UnityEngine;
using Character.Runtime;

/// <summary>
/// Place ce composant sur un GameObject avec un Collider en mode Trigger.
/// Quand le joueur entre dans la zone, il quitte la spline et peut se déplacer librement en 3D.
/// </summary>
[RequireComponent(typeof(Collider))]
public class FreeMovementZone : MonoBehaviour
{
    [Header("Paramètres de déplacement libre")]
    [Tooltip("Vitesse de déplacement dans la zone libre")]
    public float moveSpeed = 5f;

    [Tooltip("Vitesse de rotation vers la direction de déplacement")]
    public float rotationSpeed = 10f;

    [Tooltip("Applique la gravité en mode libre (nécessite un Rigidbody sur le joueur)")]
    public bool applyGravity = true;

    private void Awake()
    {
        // S'assurer que le collider est bien en mode Trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // Basculer en mode libre en transmettant les paramètres de cette zone
        player.EnterFreeMovement(moveSpeed, rotationSpeed, applyGravity);
    }

    // Optionnel : si le joueur sort de la zone sans passer par un SplineExitTrigger,
    // on pourrait forcer le retour sur la dernière spline connue.
    // Décommentez si vous voulez ce comportement de fallback.
    /*
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var player = other.GetComponent<PlayerController>();
        player?.ExitFreeMovement();
    }
    */

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.15f);
        var col = GetComponent<Collider>();
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.5f);
            Gizmos.DrawWireCube(box.center, box.size);
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
        }
    }
#endif
}
