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

        [Header("2D Metrics")]
        public float lateralSpeed = 5f; 
        public float runMultiplier = 2.18f; 
        public float acceleration = 12f;
        public float rotationSpeed = 15f; 
        
        [Header("Jump Settings")]
        public float jumpHeight = 1.2f;
        public float gravity = -15f;

        [Header("State")]
        public bool isHunted = false; 

        private CharacterController charController;
        private Vector3 currentVelocity;
        private Vector3 verticalVelocity;
        
        public bool isMovementLocked = false;
        
        void Awake()
        {
            charController = GetComponent<CharacterController>();
            attachmentManager = GetComponent<PlayerAttachmentManager>();
        }

        void Update()
        {
            if (isMovementLocked) return;

            Vector2 input = Vector2.zero;
            if (joystick != null)
            {
                input = joystick.InputDirection;
            }

            ApplyMovement2D(input);
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

        private void ApplyMovement2D(Vector2 input)
        {
            float weightMultiplier = 1f;
            if (attachmentManager != null)
            {
                weightMultiplier = attachmentManager.currentSpeed / attachmentManager.baseSpeed;
            }

            float mult = isHunted ? runMultiplier : 1f;
            
            Vector3 targetVelocity = new Vector3(input.x * lateralSpeed * mult * weightMultiplier, 0, 0);
            
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            charController.Move(currentVelocity * Time.deltaTime);
            
            if (Mathf.Abs(currentVelocity.x) > 0.01f)
            {
                float directionX = currentVelocity.x > 0 ? 1f : -1f;
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionX, 0, 0));
                
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