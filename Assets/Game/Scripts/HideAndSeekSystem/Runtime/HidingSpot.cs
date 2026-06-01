using UnityEngine;

namespace HideAndSeekSystem.Runtime
{
    public class HidingSpot : MonoBehaviour
    {
        private GameObject player;
        private bool isPlayerInside = false;
        private bool isHidden = false;
        
        [SerializeField] private Transform exitPoint;

        private void Update()
        {
            // On vérifie si l'Instance du manager existe pour éviter les crashs
            if (ShadowManager.Instance != null)
            {
                // Si le joueur est VRAIMENT caché ici, on met à jour le manager
                if (isHidden)
                {
                    ShadowManager.Instance.isPlayerHidden = true;
                }
            }

            // [OPTIONNEL] : Si tu veux pouvoir appuyer sur une touche (ex: E) pour interagir
            if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
            {
                OnInteract();
            }
        }

        public void OnInteract()
        {
            if (isPlayerInside)
            {
                if (!isHidden)
                {
                    HidePlayer();
                }
                else
                {
                    ExitHidden();
                }
            }
        }

        private void HidePlayer()
        {
            isHidden = true;
            Debug.Log("Je suis caché !");
            
            // On téléporte le joueur au centre de la cachette
            player.transform.position = transform.position;
            
            // AU LIEU DE SetActive(false), on cache le visuel et on bloque les mouvements :
            TogglePlayerComponents(false);
        }

        private void ExitHidden()
        {
            isHidden = false;
            Debug.Log("Je sors !");
            
            // On remet le manager à false dès qu'on sort
            if (ShadowManager.Instance != null)
            {
                ShadowManager.Instance.isPlayerHidden = false;
            }
            
            player.transform.position = exitPoint.position;
            
            // On réactive le visuel et les mouvements
            TogglePlayerComponents(true);
        }

        // Cette fonction permet de "désactiver" le joueur proprement sans casser les triggers
        private void TogglePlayerComponents(bool visible)
        {
            if (player == null) return;

            // Cache/Affiche le visuel 3D du joueur
            MeshRenderer[] renderers = player.GetComponentsInChildren<MeshRenderer>();
            foreach (var r in renderers) r.enabled = visible;

            // Désactive/Active le script de mouvement pour pas que le joueur puisse bouger en étant caché
            // REMPLACE "CharacterController" par ton script de mouvement (ex: PlayerController, ThirdPersonController...)
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = visible;
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
                // On ne reset l'état que si le joueur n'est pas en train de se cacher
                if (!isHidden)
                {
                    isPlayerInside = false;
                    player = null;
                }
            }
        }
    }
}