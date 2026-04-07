using UnityEngine;
using FeatherSystem.Runtime;

namespace ColorSystem.Runtime
{
    public class CampFire : MonoBehaviour
    {
        private GameObject player;
        private bool isPlayerInside = false;
        private bool isCampFire = false;
        [SerializeField] GameColorManager gameColorManager;
        private PlayerPowers playerPowers;
        
        
        public void OnInteract()
        {
            if (isPlayerInside && playerPowers.hasFireFeather)
            {
                Debug.Log("Allumé le feu");
                isCampFire = true;
            }
            else if (isPlayerInside && !playerPowers.hasFireFeather)
            {
                Debug.Log("Tu na pas la plume de feu");
            }
        }

        private void Update()
        {
            if (isCampFire && gameColorManager.currentSaturation < gameColorManager.maxSaturation)
                gameColorManager.RestoreColor(5f);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                player = other.gameObject;
                isPlayerInside = true;
                playerPowers = other.GetComponent<PlayerPowers>();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInside = false;
                isCampFire = false;                                                                                                                  
            }
        }
    }
}