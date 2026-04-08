using UnityEngine;
using Character.Runtime;

namespace Gameplay.Environment
{
    public class SplineJunction : MonoBehaviour
    {
        public UnityEngine.Splines.SplineContainer nextSpline;
        [Tooltip("Distance en mètres sur la nouvelle spline où le joueur va arriver")]
        public float entryDistance = 0f; 
        [Range(-1f, 1f)] public float threshold = 0.5f;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var _player = other.GetComponent<PlayerController>();
            
                if ((threshold > 0 && _player.VerticalInput > threshold) ||
                    (threshold < 0 && _player.VerticalInput < threshold))
                {
                    _player.SwitchSpline(nextSpline, entryDistance); 
                    Debug.Log("Changement vers " + nextSpline.name + " à la distance " + entryDistance);
                }
            }
        }
    }
}