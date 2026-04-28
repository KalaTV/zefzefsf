using UnityEngine;
using Character.Runtime;
using UnityEngine.Splines;

public class SplineJunction : MonoBehaviour
{
    public SplineContainer targetSpline;
    [Header("Fluidity Settings")]
    
    [Range(-1f, 1f)] public float directionTrigger = 0.5f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            
            if (player.activeSpline == targetSpline) return;
            
            if ((directionTrigger > 0 && player.SideInput > 0.5f) || 
                (directionTrigger < 0 && player.SideInput < -0.5f))
            {
                player.SwitchSpline(targetSpline);
            }
        }
    }
}