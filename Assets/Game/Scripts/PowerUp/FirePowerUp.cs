using UnityEngine;
using UnityEngine.EventSystems;
using FeatherSystem.Runtime; 

namespace FeatherCollectibleSystem.Runtime
{
    public class FirePowerUp : MonoBehaviour, IPointerClickHandler
    {
        [Header("Configuration")]
        [SerializeField] private float maxReachDistance = 3.5f; 
        [SerializeField] private GameObject pickupEffect;
    
        public void OnPointerClick(PointerEventData eventData)
        {
            PlayerPowers powers = Object.FindFirstObjectByType<PlayerPowers>(); 
            
            if (powers == null)
            {
                Debug.LogError("Le script PlayerPowers n'a pas été trouvé dans la scène !");
                return;
            }
            
            float distance = Vector3.Distance(powers.transform.position, transform.position);

            if (distance <= maxReachDistance)
            {
                powers.hasFireFeather = true;
                Debug.Log("Pouvoir de feu activé ! Booléen passé à true.");
                
                if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Trop loin pour ramasser la plume. Distance actuelle : " + distance);
            }
        }
    }
}