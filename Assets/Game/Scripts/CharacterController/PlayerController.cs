using UnityEngine;
using PinePie.SimpleJoystick;
using EnemyAttachmentSystem.Runtime;

namespace Character.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private JoystickController joystick;
        private PlayerAttachmentManager attachmentManager;

        [Header("2.5D Metrics")]
        public float lateralSpeed = 5f; 
        public float depthSpeed = 3f;   
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
            
            attachmentManager = GetComponent<PlayerAttachmentManager>();
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
            bool canJump = true;
            if(attachmentManager != null && attachmentManager.currentAttachedCount > 3) canJump = false;

            if (charController.isGrounded && canJump)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private void ApplyMovement25D(Vector2 input)
        {
            Vector3 camForward = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(camTransform.right, Vector3.up).normalized;
            
            float weightMultiplier = 1f;
            if (attachmentManager != null)
            {
                weightMultiplier = attachmentManager.currentSpeed / attachmentManager.baseSpeed;
            }

            float mult = isHunted ? runMultiplier : 1f;
            
            Vector3 targetVelocity = ((camRight * input.x * lateralSpeed * mult) + 
                                     (camForward * input.y * depthSpeed * mult)) * weightMultiplier;
            
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