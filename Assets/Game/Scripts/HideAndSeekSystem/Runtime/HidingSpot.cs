using System;
using UnityEngine;

namespace HideAndSeekSystem.Runtime
{
    public class HidingSpot : MonoBehaviour
    {
        private GameObject player;
        private bool isPlayerInside = false;
        private bool isHidden = false;
        
        [SerializeField] private Transform exitPoint;
        [SerializeField] private ShadowManager shadowManager;

        private void FixedUpdate()
        {
            shadowManager.isPlayerHidden = isHidden;
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
            Debug.Log("je suis caché");
            
            
            player.transform.position = transform.position;
            
            player.SetActive(false);
        }

        private void ExitHidden()
        {
            isHidden = false;
            Debug.Log("je sort");
            
            player.transform.position = exitPoint.position;
            
            player.SetActive(true);
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
            }
        }
    }
}