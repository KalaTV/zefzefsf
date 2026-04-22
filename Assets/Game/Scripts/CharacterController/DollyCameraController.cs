using UnityEngine;

namespace Character.Runtime
{
    public class DollyCameraController : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;

        [Header("Position Settings")]
        public Vector3 offset = new Vector3(0f, 4f, -8f);
        public float smoothSpeed = 0.125f;

        [Header("Constraints")]
        public bool limitZ = true;
        public float minZ = -10f;
        public float maxZ = 2f;

        private Vector3 _currentVelocity = Vector3.zero;

        void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;

            if (limitZ)
            {
                desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);
            }

            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothSpeed);
            
            transform.position = smoothedPosition;
        }
    }
}