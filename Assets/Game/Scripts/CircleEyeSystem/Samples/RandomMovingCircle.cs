using UnityEngine;

namespace CircleEyeSystem.Runtime
{
    public class RandomMovingCircle : MonoBehaviour
    {
        public CircleSettings settings;

        [Header("Pupille distante")]
        [Tooltip("L'IrisController de l'œil qui regarde ce cercle. Peut être n'importe où dans la scène.")]
        public IrisController pupil;

        private float targetPosition;
        private Vector3 origin;
        private bool wasDetected = false;

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
            Vector3 targetFullPos = new Vector3(targetPosition, origin.y, origin.z);

            transform.position = Vector3.MoveTowards(
                transform.position, targetFullPos, settings.moveSpeed * Time.deltaTime
            );

            if (Mathf.Abs(transform.position.x - targetPosition) < 0.01f)
                SetNextTarget();
        }

        private void SetNextTarget()
        {
            float randomOffset = Random.Range(-settings.wanderRadius, settings.wanderRadius);
            targetPosition = origin.x + randomOffset;
        }

        private void CheckDetection()
        {
            bool detected = Physics.CheckSphere(
                transform.position, settings.detectionRadius, settings.playerLayer
            );

            if (detected && !wasDetected)
            {
                Debug.Log("<color=yellow>Joueur détecté !</color>");
                pupil?.OnPlayerDetected();
            }
            else if (!detected && wasDetected)
            {
                Debug.Log("<color=cyan>Joueur perdu.</color>");
                pupil?.OnPlayerLost();
            }

            wasDetected = detected;
        }

        private void OnDrawGizmos()
        {
            if (settings == null) return;

            Vector3 currentOrigin = Application.isPlaying ? origin : transform.position;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                currentOrigin + Vector3.left  * settings.wanderRadius,
                currentOrigin + Vector3.right * settings.wanderRadius
            );

            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, settings.detectionRadius);
        }
    }
}