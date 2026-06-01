using UnityEngine;

namespace ColorSystem.Runtime
{
    public class CampFire : MonoBehaviour
    {
        private bool isPlayerInside = false;
        public bool isCampFire = false;

        [SerializeField] private GameColorManager gameColorManager;
        [SerializeField] private GameObject fireVFX;

        public void OnInteract()
        {
            if (isPlayerInside)
            {
                isCampFire = true;
                Debug.Log("Allumé le feu");
            }
        }

        private void Update()
        {
            if (fireVFX != null)
                fireVFX.SetActive(isCampFire);

            if (isCampFire && gameColorManager.currentSaturation < gameColorManager.maxSaturation && isPlayerInside)
                gameColorManager.RestoreColor(5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                isPlayerInside = true;
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