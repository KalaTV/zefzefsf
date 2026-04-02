using System;
using UnityEngine;

namespace CircleEyeSystem.Runtime
{
    public class RandomMovingCircle : MonoBehaviour
    {
        public CircleSettings settings;

        private float targetPosition;
        private Vector3 origin;

        void Start()
        {
            origin = transform.position;
            SetNextTarget();
        }

        void Update()
        {
            Move();
            CheckDetection();
        }

        private void Move()
        {
            Vector3 TargetFullPos = new Vector3(targetPosition, origin.y, origin.z);
            
            transform.position = Vector3.MoveTowards(transform.position, TargetFullPos, settings.moveSpeed * Time.deltaTime);
            
            if (Mathf.Abs(transform.position.x - targetPosition) < 0.01f)
            {
                SetNextTarget();
            }
        }

        private void SetNextTarget()
        {
            float randomOffset = UnityEngine.Random.Range(-settings.wanderRadius, settings.wanderRadius);
            targetPosition = origin.x + randomOffset;
        }

        private void CheckDetection()
        {
            if (Physics.CheckSphere(transform.position, settings.detectionRadius, settings.playerLayer))
            {
                Debug.Log("<color=yellow>Joueur détecté sur l'axe X !</color>");
            }
        }
        
        private void OnDrawGizmos()
        {
            if (settings == null) return;

            Vector3 currentOrigin = Application.isPlaying ? origin : transform.position;
            
            Gizmos.color = Color.red;
            Vector3 lineStart = currentOrigin + Vector3.left * settings.wanderRadius;
            Vector3 lineEnd = currentOrigin + Vector3.right * settings.wanderRadius;
            Gizmos.DrawLine(lineStart, lineEnd);

            
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawWireSphere(transform.position, settings.detectionRadius);
        }

    }
}