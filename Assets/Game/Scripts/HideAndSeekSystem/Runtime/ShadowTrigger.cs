using UnityEngine;

namespace HideAndSeekSystem.Runtime
{
    public class ShadowTrigger : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private bool destroyOnTrigger = true;

        [Header("Local Spawn Settings")]
        [Tooltip("Le point d'apparition des ombres POUR CETTE ZONE spécifique")]
        [SerializeField] private Transform localSpawnPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                ShadowManager manager = Object.FindFirstObjectByType<ShadowManager>();
                
                if (manager != null)
                {
                    if (!manager.IsEventActive) 
                    {
                        // 1. On donne le point d'apparition local au manager AVANT de lancer l'event
                        manager.SetSpawnPoint(localSpawnPoint);

                        // 2. On lance l'événement
                        manager.StartShadowEvent();
                        
                        if (destroyOnTrigger)
                        {
                            Destroy(gameObject); 
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("ShadowTrigger : Aucun ShadowManager trouvé !");
                }
            }
        }
    }
}