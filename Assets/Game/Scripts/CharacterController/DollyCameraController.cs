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
        
        [Header("Transition Settings")]
        public float offsetTransitionSpeed = 2f;

        [Header("Constraints")]
        public bool limitZ = true;
        public float minZ = -10f;
        public float maxZ = 2f;

        private Vector3 _currentVelocity = Vector3.zero;
        private Vector3 _defaultOffset;
        private Vector3 _targetOffset;

        private bool _isLocked = false;
        private Vector3 _lockedPosition;

        void Start()
        {
            _defaultOffset = offset;
            _targetOffset = offset;
        }

        public void SetNewOffset(Vector3 newOffset)
        {
            _targetOffset = newOffset;
        }

        public void ResetOffset()
        {
            _targetOffset = _defaultOffset;
        }

        public void LockCamera(Vector3 fixedWorldPosition)
        {
            _isLocked = true;
            _lockedPosition = fixedWorldPosition;
        }

        public void UnlockCamera()
        {
            _isLocked = false;
        }

        void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition;

            if (_isLocked)
            {
                desiredPosition = _lockedPosition;
            }
            else
            {
                offset = Vector3.Lerp(offset, _targetOffset, Time.deltaTime * offsetTransitionSpeed);
                desiredPosition = target.position + offset;
                
                if (limitZ)
                {
                    desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);
                }
            }

            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}