using UnityEngine;

namespace Character.Runtime
{
    public class CameraZone : MonoBehaviour
    {
        private DollyCameraController _camController;

        [Header("Mode de Zone")]
        public bool lockCameraPosition = false;

        [Header("Si 'Lock Camera' est COCHÉ")]
        public Transform fixedCameraPoint;

        [Header("Si 'Lock Camera' est DÉCOCHÉ")]
        public Vector3 zoneOffset = new Vector3(0f, 10f, -15f);

        void Start()
        {
            _camController = Object.FindFirstObjectByType<DollyCameraController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _camController != null)
            {
                if (lockCameraPosition)
                {
                    if (fixedCameraPoint != null)
                    {
                        _camController.LockCamera(fixedCameraPoint.position);
                    }
                    else
                    {
                        _camController.LockCamera(_camController.transform.position); 
                    }
                }
                else
                {
                    _camController.SetNewOffset(zoneOffset);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && _camController != null)
            {
                _camController.UnlockCamera();
                _camController.ResetOffset();
            }
        }
    }
}