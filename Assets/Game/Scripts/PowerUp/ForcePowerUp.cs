using UnityEngine;
using UnityEngine.EventSystems;

namespace FeatherCollectibleSystem.Runtime
{
    public class ForcePowerUp : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI")]
        public GameObject grabButton;
        
        [Header("Configuration")]
        [SerializeField] private float maxReachDistance = 3.5f; 
        [SerializeField] private GameObject pickupEffect;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            
            PlayerPushPullController module = player.GetComponent<PlayerPushPullController>();
            
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= maxReachDistance)
            {
                if (module != null)
                {
                    module.enabled = true;
                    
                    Debug.Log("Script de poussée activé !");
                    if (grabButton != null)
                    {
                        grabButton.SetActive(true);
                    }
                    
                    if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.Log("Trop loin");
            }
        }
    }
}
