using UnityEngine;
using Character.Runtime;

namespace Character.Runtime
{
    public class CameraAxisTrigger : MonoBehaviour
    {
        [Header("Camera Reference")]
        public DollyCameraController dollyCam;

        [Header("Player Reference")]
        public PlayerController player;

        [Header("New Offset on Enter")]
        public Vector3 newOffset = new Vector3(-8f, 4f, 0f);

        [Header("Player Rotation on Enter")]
        public float newPlayerRotationY = 90f;
        private float _previousRotationY;

        [Header("Reset on Exit ?")]
        public bool resetOnExit = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (dollyCam != null)
                dollyCam.SetNewOffset(newOffset);

            if (player != null)
            {
                _previousRotationY = player.transform.eulerAngles.y;
                player.transform.rotation = Quaternion.Euler(0f, newPlayerRotationY, 0f);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (resetOnExit && dollyCam != null)
                dollyCam.ResetOffset();

            if (resetOnExit && player != null)
                player.transform.rotation = Quaternion.Euler(0f, _previousRotationY, 0f);
        }
    }
}