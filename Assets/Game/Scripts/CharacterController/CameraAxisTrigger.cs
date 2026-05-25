using UnityEngine;
using Character.Runtime;

namespace Character.Runtime
{
    public class CameraAxisTrigger : MonoBehaviour
    {
        [Header("Camera Reference")]
        public DollyCameraController dollyCam;

        [Header("New Offset on Enter")]
        public Vector3 newOffset = new Vector3(-8f, 4f, 0f); // axe X par défaut

        [Header("Reset on Exit ?")]
        public bool resetOnExit = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (dollyCam != null)
                dollyCam.SetNewOffset(newOffset);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (resetOnExit && dollyCam != null)
                dollyCam.ResetOffset();
        }
    }
}