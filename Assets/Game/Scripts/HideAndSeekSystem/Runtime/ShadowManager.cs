using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace HideAndSeekSystem.Runtime
{
    public class ShadowManager : MonoBehaviour
    {
        [Header("Timer Settings")]
        [SerializeField] private float timeToHide = 60f;
        private float currentTime;
        private bool isEventActive = false;
        
        [Header("UI Settings")]
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Shadow Settings")]
        [SerializeField] private GameObject[] shadowPrefabs;
        [SerializeField] private Transform spawnPoint;
        [Tooltip("How many shadows will be shown")]
        [SerializeField] private int numberOfShadowsToSpawn = 5;
        [Tooltip("Time in seconds before shadows will be destroyed")]
        [SerializeField] private float passageDuration = 3f;
        
        [HideInInspector]
        public bool isPlayerHidden;
        private bool shadowsArrived = false;

        private void Start()
        {
            if(uiPanel != null) uiPanel.SetActive(false);
            isEventActive = false;
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
                if (shadowPrefabs.Length > 0 && spawnPoint != null)
                {
                    Vector3 randomOffset = new Vector3(
                        UnityEngine.Random.Range(-2f, 2f), 
                        0,                                 
                        UnityEngine.Random.Range(-1f, 1f)  
                    );

                    Vector3 finalSpawnPos = spawnPoint.position + randomOffset;

                    int randomIndex = UnityEngine.Random.Range(0, shadowPrefabs.Length);
                    Instantiate(shadowPrefabs[randomIndex], finalSpawnPos, spawnPoint.rotation);
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
                warningText.text = "Tu a Survécu !";
            }
            else
            {
                warningText.text = "Game Over !";
            }
        }
        private void ResetManager()
        {
            isEventActive = false;
            shadowsArrived = false; 
    
            if(uiPanel != null) uiPanel.SetActive(false);
    
            
        }
    }
}