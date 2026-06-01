using UnityEngine;
using EnemyAttachmentSystem.Runtime;

namespace EnemyAttachmentSystem.Samples
{
    public class CleanerItem : MonoBehaviour
    {
        [Header("Effets visuels (Optionnel)")]
        [SerializeField] private GameObject collectParticles;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerAttachmentManager manager = other.GetComponent<PlayerAttachmentManager>();

                if (manager != null)
                {
                    
                    if (manager.currentAttachedCount > 0)
                    {
                        manager.DestroyAllAttachedEnemies();
                        
                        if (collectParticles != null)
                        {
                            Instantiate(collectParticles, transform.position, Quaternion.identity);
                        }
                        
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.Log("Pas d'ennemis à détruire, le bonus reste en place.");
                    }
                }
            }
        }
    }
}