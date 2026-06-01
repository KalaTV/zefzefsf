using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Character.Runtime; // Permet au manager de reconnaître le PlayerController
using Gameplay.UI;
using Object = UnityEngine.Object;

namespace HideAndSeekSystem.Runtime
{
    public class ShadowManager : MonoBehaviour
    {
        [Header("Timer Settings")]
        [SerializeField] private float timeToHide = 60f;
        private float currentTime;
        private bool isEventActive = false;
        
        public bool IsEventActive => isEventActive;

        [Header("UI Settings")]
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Shadow Settings")]
        [SerializeField] private GameObject[] shadowPrefabs;
        [SerializeField] private int numberOfShadowsToSpawn = 5;
        [SerializeField] private float passageDuration = 3f;
        
        // Ce n'est plus un [SerializeField], il est défini dynamiquement par le trigger !
        private Transform currentSpawnPoint; 

        [HideInInspector]
        public bool isPlayerHidden;
        private bool shadowsArrived = false;

        public static ShadowManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        private void Start()
        {
            if(uiPanel != null) uiPanel.SetActive(false);
            isEventActive = false;
        }

        // Cette fonction permet au trigger de donner sa position au manager
        public void SetSpawnPoint(Transform newSpawnPoint)
        {
            currentSpawnPoint = newSpawnPoint;
        }
        
        public void StartShadowEvent()
        {
            if (isEventActive) return; 

            isEventActive = true;
            currentTime = timeToHide;
            
            if(uiPanel != null) uiPanel.SetActive(true);
            if(warningText != null) warningText.text = "VITE ! TROUVE UNE CACHETTE !"; 
        }

        private void Update()
        {
            if (!isEventActive) return; 

            currentTime -= Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(currentTime).ToString() + "s";

            if (currentTime <= 0)
            {
                isEventActive = false; 
                StartCoroutine(ShadowSequence());
            }
        }

        private IEnumerator ShadowSequence()
        {
            shadowsArrived = true;
            if (timerText != null) timerText.text = "0s";
            if (warningText != null) warningText.text = "Chut... elles passent...";

            float timeBetweenShadows = passageDuration / Mathf.Max(1, numberOfShadowsToSpawn);

            for (int i = 0; i < numberOfShadowsToSpawn; i++)
            {
                // On utilise currentSpawnPoint reçu du trigger
                if (shadowPrefabs.Length > 0 && currentSpawnPoint != null)
                {
                    Vector3 randomOffset = new Vector3(
                        UnityEngine.Random.Range(-2f, 2f), 
                        0,                                 
                        UnityEngine.Random.Range(-1f, 1f)  
                    );

                    Vector3 finalSpawnPos = currentSpawnPoint.position + randomOffset;

                    int randomIndex = UnityEngine.Random.Range(0, shadowPrefabs.Length);
                    Instantiate(shadowPrefabs[randomIndex], finalSpawnPos, currentSpawnPoint.rotation);
                }
                
                float randomWait = timeBetweenShadows * UnityEngine.Random.Range(0.5f, 1.5f);
                yield return new WaitForSeconds(randomWait);
            }

            yield return new WaitForSeconds(2.0f);
            CheckWinCondition();
            yield return new WaitForSeconds(3.0f);
            ResetManager();
        }

        private void CheckWinCondition()
        {
            if (isPlayerHidden)
            {
                if (warningText != null) warningText.text = "Tu as survécu !";
            }
            else
            {
                if (warningText != null) warningText.text = "Game Over !";
        
                // On cherche le joueur dans la scène pour le tuer
                PlayerController player = Object.FindFirstObjectByType<PlayerController>();
                if (player != null)
                {
                    // On lance la routine de mort directement depuis le manager
                    StartCoroutine(ShadowDeathRoutine(player));
                }
                else
                {
                   
                }
            }
        }

// Routine copiée sur ton script DeathZone
        private IEnumerator ShadowDeathRoutine(PlayerController player)
        {
            player.isMovementLocked = true;
            
            player.Respawn();

            yield return new WaitForSeconds(0.2f);

            player.isMovementLocked = false;
        }

        private void ResetManager()
        {
            isEventActive = false;
            shadowsArrived = false; 
            currentSpawnPoint = null; // On nettoie le point de spawn
    
            if(uiPanel != null) uiPanel.SetActive(false);
        }
    }
}