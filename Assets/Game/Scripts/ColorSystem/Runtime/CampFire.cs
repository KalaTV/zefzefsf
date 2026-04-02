using UnityEngine;

namespace ColorSystem.Runtime
{
    public class CampFire : MonoBehaviour
    {
        private GameObject player;
        private bool isPlayerInside = false;
        private bool isCampFire = false;
        [SerializeField] GameColorManager gameColorManager;

        public void OnInteract()
        {
            if (isPlayerInside)
            {
                Debug.Log("Allumé le feu");
                isCampFire = true;
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