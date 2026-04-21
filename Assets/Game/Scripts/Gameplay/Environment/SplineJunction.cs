using UnityEngine;
using Character.Runtime;
using UnityEngine.Splines;

public class SplineJunction : MonoBehaviour
{
    public SplineContainer targetSpline;
    
    [Tooltip("Pousser le stick vers : 0.5 = Haut/Droite, -0.5 = Bas/Gauche")]
    [Range(-1f, 1f)] public float directionTrigger = 0.5f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var p = other.GetComponent<PlayerController>();
            
            // SÉCURITÉ ANTI-BOUCLE : Si on est déjà sur le nouveau rail, on bloque !
            if (p.activeSpline == targetSpline) return;
            
            // On vérifie si tu pousses le stick du bon côté
            if ((directionTrigger > 0 && p.SideInput > 0.5f) || 
                (directionTrigger < 0 && p.SideInput < -0.5f))
            {
                p.SwitchSpline(targetSpline);
            }
        }
    }
}