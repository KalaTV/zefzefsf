using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character.Runtime;          // REQUIS : Pour le PlayerController et Respawn
using Gameplay.UI;               // REQUIS : Pour le ScreenFader

namespace EnemyAttachmentSystem.Runtime
{
    public class PlayerAttachmentManager : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private List<Transform> attachmentPoints;
        private List<Transform> availablePoints;
        public int currentAttachedCount { get; set; }
        
        [Header("Speed")]
        public float baseSpeed = 5f;
        [HideInInspector] public float currentSpeed;
        
        private bool isDead = false;

        private void Start()
        {
            availablePoints = new List<Transform>(attachmentPoints);
            UpdateSpeed();
        }
        
        public Transform GetRandomAttachmentPoint()
        {
            if (isDead || availablePoints.Count == 0) return null;
            
            int randomIndex = UnityEngine.Random.Range(0, availablePoints.Count);
            Transform chosenPoint = availablePoints[randomIndex];
            
            availablePoints.RemoveAt(randomIndex);
            
            currentAttachedCount++;
            UpdateSpeed();
            
            if (currentAttachedCount >= attachmentPoints.Count)
            {
                Die();
            }

            return chosenPoint;
        }
        
        private void UpdateSpeed()
        {
            float penaltyPercent = (float)currentAttachedCount / attachmentPoints.Count;
            currentSpeed = baseSpeed * (1f - penaltyPercent);
        }
        
        private void Die()
        {
            if (isDead) return; // Sécurité anti-boucle
            
            isDead = true;
            currentSpeed = 0f;
            Debug.Log("GAME OVER : Trop d'ennemis accrochés !");

            // On cherche le PlayerController sur ce GameObject (ou ses enfants/parents)
            PlayerController player = GetComponentInParent<PlayerController>();
            if (player == null) player = GetComponentInChildren<PlayerController>();

            if (player != null)
            {
                // On lance la même routine de mort que la DeathZone vers le Checkpoint
                StartCoroutine(AttachmentDeathRoutine(player));
            }
            else
            {
                Debug.LogError("PlayerAttachmentManager : Impossible de lancer la mort, PlayerController introuvable !");
            }
        }

        private IEnumerator AttachmentDeathRoutine(PlayerController player)
        {
            // 1. On fige le joueur
            player.isMovementLocked = true;
            
            
            
            // 3. IMPORTANT : On nettoie tous les ennemis accrochés pendant que l'écran est noir !
            DestroyAllAttachedEnemies();
            
            // 4. Téléportation au dernier checkpoint sauvegardé
            player.Respawn();

            yield return new WaitForSeconds(0.2f);
            

            player.isMovementLocked = false;
        }
        
        public void DestroyAllAttachedEnemies()
        {
            if (currentAttachedCount == 0) return;
            
            foreach (Transform point in attachmentPoints)
            {
                if (point.childCount > 0)
                {
                    Destroy(point.GetChild(0).gameObject);
                }
            }
            
            availablePoints = new List<Transform>(attachmentPoints);
            
            currentAttachedCount = 0;
            isDead = false; // Permet de rejouer après la mort
            UpdateSpeed();
            
            Debug.Log("Tous les ennemis ont été détruits !");
        }
    }
}