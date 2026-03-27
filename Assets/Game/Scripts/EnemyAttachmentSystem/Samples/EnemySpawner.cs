using UnityEngine;
using System.Collections;

namespace EnemyAttachmentSystem.Samples
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefabs;
        [SerializeField] private Transform spawnPoint;

        public void SpawnEnemy()
        {
            Instantiate(enemyPrefabs, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
