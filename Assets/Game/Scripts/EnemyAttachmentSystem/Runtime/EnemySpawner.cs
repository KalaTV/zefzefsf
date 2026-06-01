using UnityEngine;
using System.Collections;

namespace EnemyAttachmentSystem.Samples
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private GameObject enemyPrefab; 
        [SerializeField] private Transform spawnPoint;

        [Header("Paramètres d'apparition")]
        [SerializeField] private int maxEnemies = 5;
        [SerializeField] private float spawnInterval = 3f;

        private int _spawnCount = 0;
        private bool _isSpawning = false; 

        private void OnTriggerEnter(Collider other)
        {
            
            if (other.CompareTag("Player") && !_isSpawning && _spawnCount < maxEnemies)
            {
                StartCoroutine(SpawnEnemyRoutine());
            }
        }

        private IEnumerator SpawnEnemyRoutine()
        {
            _isSpawning = true; 

            
            while (_spawnCount < maxEnemies)
            {
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
          
                _spawnCount++;
                
                yield return new WaitForSeconds(spawnInterval);
            }
            
            _isSpawning = false; 
            
            Debug.Log("Les 5 ennemis ont été générés !");
        }
    }
}