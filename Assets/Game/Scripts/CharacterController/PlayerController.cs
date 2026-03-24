using UnityEngine;
using PinePie.SimpleJoystick;

namespace Character.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private JoystickController joystick;

        [Header("2.5D Metrics")]
        public float lateralSpeed = 2.2f; 
        public float depthSpeed = 1.5f;   
        public float runMultiplier = 2.18f; 
        public float acceleration = 12f;
        public float rotationSpeed = 15f; 
        
        [Header("Jump Settings")]
        public float jumpHeight = 1.2f;
        public float gravity = -15f;

        [Header("State")]
        public bool isHunted = false; 

        private CharacterController charController;
        private Transform camTransform;
        private Vector3 currentVelocity;
        private Vector3 verticalVelocity;

        void Awake()
        {
            charController = GetComponent<CharacterController>();
            camTransform = Camera.main.transform;
        }

        void Update()
        {
            Vector2 input = Vector2.zero;
            if (joystick != null)
            {
                input = joystick.InputDirection;
            }

            ApplyMovement25D(input);
            ApplyGravity();
        }
        
        public void Jump()
        {
            if (charController.isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private void ApplyMovement25D(Vector2 input)
        {
            Vector3 camForward = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(camTransform.right, Vector3.up).normalized;

            float mult = isHunted ? runMultiplier : 1f;
            Vector3 targetVelocity = (camRight * input.x * lateralSpeed * mult) + (camForward * input.y * depthSpeed * mult);
            
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            charController.Move(currentVelocity * Time.deltaTime);
            
            if (currentVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentVelocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private void ApplyGravity()
        {
            if (charController.isGrounded && verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f; 
            }

            verticalVelocity.y += gravity * Time.deltaTime;
            charController.Move(verticalVelocity * Time.deltaTime);
        }
    }
}