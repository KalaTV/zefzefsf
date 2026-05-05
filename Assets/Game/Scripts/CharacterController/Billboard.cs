using UnityEngine;

namespace Character.Runtime
{
    public class Billboard : MonoBehaviour
    {
        [Header("Ajustements")]
        
        [SerializeField] private float rotationOffsetY = 180f; 

       
        [SerializeField] private bool lockVerticalAxis = true;

        private Transform cameraTransform;

        void Start()
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        void LateUpdate()
        {
            if (cameraTransform == null) return;
            
            transform.LookAt(cameraTransform.position, Vector3.up);

           
            transform.Rotate(0f, rotationOffsetY, 0f, Space.Self);
            
            if (lockVerticalAxis)
            {
                Vector3 currentEuler = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(0f, currentEuler.y, 0f);
            }
        }
    }
}