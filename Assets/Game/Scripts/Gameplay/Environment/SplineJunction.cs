using UnityEngine;
using Character.Runtime;
using UnityEngine.Splines;

/// <summary>
/// Gère les transitions directes spline → spline (sans zone libre).
/// Pour les transitions spline → zone libre → spline,
/// utilise FreeMovementZone + SplineExitTrigger à la place.
/// </summary>
public class SplineJunction : MonoBehaviour
{
    public SplineContainer targetSpline;

    [Header("Fluidity Settings")]
    [Tooltip("Direction du joystick requise pour switcher. " +
             "> 0 = joystick à droite, < 0 = joystick à gauche, 0 = toujours switcher")]
    [Range(-1f, 1f)] public float directionTrigger = 0.5f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // Ne rien faire si le joueur est en mode libre
        if (player.MovementMode == PlayerMovementMode.Free) return;

        // Déjà sur cette spline
        if (player.activeSpline == targetSpline) return;

        bool shouldSwitch = directionTrigger == 0f
                            || (directionTrigger > 0 && player.SideInput > 0.5f)
                            || (directionTrigger < 0 && player.SideInput < -0.5f);

        if (shouldSwitch)
            player.SwitchSpline(targetSpline);
    }
}