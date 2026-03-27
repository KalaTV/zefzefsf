using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAttachmentSystem.Runtime
{
    public class PlayerAttachmentManager : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private List<Transform> attachmentPoints;
        private List<Transform> availablePoints;
        public int currentAttachedCount { get; private set; }
        
        
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
            isDead = true;
            currentSpeed = 0f;
            Debug.Log("GAME OVER : Trop d'ennemis accrochés !");
        }
    }
}
