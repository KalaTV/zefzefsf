using UnityEngine;

namespace EnemyAttachmentSystem.Runtime
{
    public class AttachableEnemy : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float moveSpeed = 3f;
        
        private Transform playerTransform;
        private PlayerAttachmentManager playerManager;
        private bool isAttached = false;

        private void Update()
        {
            if (isAttached) return;
            
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            transform.LookAt(playerTransform);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isAttached) return;
            
            PlayerAttachmentManager manager = other.GetComponent<PlayerAttachmentManager>();
            
            if (manager != null)
            {
                Transform targetPoint = manager.GetRandomAttachmentPoint();
                
                if (targetPoint != null)
                {
                    GrabPoint(targetPoint);
                }
                else 
                {
                    Debug.Log("Plus de place sur le joueur !");
                }
            }
        }

        private void GrabPoint(Transform attachmentPoint)
        {
            isAttached = true;
            transform.SetParent(attachmentPoint);
            transform.localPosition = Vector3.zero; 
            transform.localRotation = Quaternion.identity; 
            
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }
}