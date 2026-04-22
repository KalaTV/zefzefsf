using UnityEngine;
using System.Collections;
using Character.Runtime;
using Gameplay.UI;

namespace Gameplay.Environment
{
    public class TeleportPoint : MonoBehaviour
    {
        [Header("References")]
        public PlayerController player; // On le glisse directement ici !
        public Transform destination;

        [Header("Settings")]
        public float interactionRange = 4f;
        public float detectionRange = 8f;
        
        [Header("Visuals")]
        public GameObject arrowVisual;

        private bool _isTeleporting = false;

        private void Awake()
        {
            if (arrowVisual != null) arrowVisual.SetActive(false);
            
            if (player == null) Debug.LogError("⚠️ Glisse le Player dans l'inspecteur de " + name);
            if (destination == null) Debug.LogError("⚠️ Glisse la Destination dans l'inspecteur de " + name);
        }

        private void Update()
        {
            if (player == null || _isTeleporting) return;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            
            if (arrowVisual != null)
            {
                arrowVisual.SetActive(distance <= detectionRange);
            }
        }

        public void ExecuteTeleport()
        {
            if (_isTeleporting || player == null) return;
            
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= interactionRange)
            {
                StartCoroutine(TeleportRoutine());
            }
            else
            {
                Debug.LogWarning("Trop loin pour se TP !");
            }
        }

        private IEnumerator TeleportRoutine()
        {
            _isTeleporting = true;
            
            player.isMovementLocked = true;

            // FADE OUT
            if (ScreenFader.Instance != null)
                yield return ScreenFader.Instance.FadeOut();

            // TELEPORT
            CharacterController charController = player.GetComponent<CharacterController>();
            if (charController != null)
            {
                charController.enabled = false;
                player.transform.position = destination.position;
                charController.enabled = true;
            }

            yield return new WaitForSeconds(0.5f);

            // FADE IN
            if (ScreenFader.Instance != null)
                yield return ScreenFader.Instance.FadeIn();

            player.isMovementLocked = false;
            _isTeleporting = false;
        }
    }
}