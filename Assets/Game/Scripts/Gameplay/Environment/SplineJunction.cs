using UnityEngine;
using Character.Runtime;
using UnityEngine.Splines;

namespace Gameplay.Environment
{
    public class SplineJunction : MonoBehaviour
    {
        [Header("Route")]
        public SplineContainer nextSpline;

        [Header("Sensibilité du Joystick")]
        [Range(-1f, 1f)] 
        [Tooltip("0.5 = Pousser vers le HAUT | -0.5 = Pousser vers le BAS")]
        public float threshold = 0.5f;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                
                if (player == null) return;
                
                bool inputMatch = false;
                
                if (threshold > 0 && player.VerticalInput > threshold) inputMatch = true;
                else if (threshold < 0 && player.VerticalInput < threshold) inputMatch = true;

                if (inputMatch)
                {
                    player.SwitchSpline(nextSpline); 
                   
                    Debug.Log($"Passage sur : {nextSpline.name}");
                }
            }
        }
    }
}