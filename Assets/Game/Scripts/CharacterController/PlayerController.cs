using UnityEngine;
using UnityEngine.Splines;
using PinePie.SimpleJoystick;
using EnemyAttachmentSystem.Runtime;

namespace Character.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private JoystickController joystick;
        [SerializeField] private SplineContainer activeSpline;
        private PlayerAttachmentManager attachmentManager;
        private SpriteRenderer _spriteRenderer;
        private CharacterController charController;

        [Header("Movement Settings")]
        public float speed = 5f;
        public float acceleration = 12f;
        public float runMultiplier = 2.18f;

        [Header("Jump Settings")]
        public float jumpHeight = 1.2f;
        public float gravity = -15f;

        [Header("State")]
        public bool isMovementLocked = false;
        public bool isHunted = false;

        private float _currentDistance = 0f;
        private float _splineLength;
        private float _currentSpeedValue;
        private Vector3 _verticalVelocity;
        
        public float VerticalInput { get; private set; }

        void Awake()
        {
            charController = GetComponent<CharacterController>();
            attachmentManager = GetComponent<PlayerAttachmentManager>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            if (activeSpline != null)
                _splineLength = activeSpline.CalculateLength();
        }

        void Update()
        {
            if (isMovementLocked || activeSpline == null) return;

            Vector2 input = (joystick != null) ? joystick.InputDirection : Vector2.zero;
            VerticalInput = input.y;

            ApplyMovementOnSpline(input.x);
        }

        private void ApplyMovementOnSpline(float inputX)
        {
            // 1. Calcul de la vitesse au sol
            float weightMultiplier = (attachmentManager != null) ? attachmentManager.currentSpeed / attachmentManager.baseSpeed : 1f;
            float mult = isHunted ? runMultiplier : 1f;
            float targetSpeed = inputX * speed * mult * weightMultiplier;

            _currentSpeedValue = Mathf.Lerp(_currentSpeedValue, targetSpeed, acceleration * Time.deltaTime);
            _currentDistance += _currentSpeedValue * Time.deltaTime;
            _currentDistance = Mathf.Clamp(_currentDistance, 0f, _splineLength);

            // 2. Calculer le mouvement HORIZONTAL (Spline)
            float t = _currentDistance / _splineLength;
            Vector3 targetSplinePos = activeSpline.EvaluatePosition(t);
            // On ignore la hauteur (Y) de la spline pour laisser le saut gérer le Y
            Vector3 horizontalMove = targetSplinePos - transform.position;
            horizontalMove.y = 0; 

            // 3. Calculer le mouvement VERTICAL (Saut/Gravité)
            ApplyGravity();

            // 4. COMBINER ET APPLIQUER
            // On combine le vecteur de la spline et le vecteur vertical
            charController.Move(horizontalMove + (_verticalVelocity * Time.deltaTime));

            // 5. Visuels
            if (Mathf.Abs(inputX) > 0.1f)
            {
                _spriteRenderer.flipX = (inputX < 0);
                
                Vector3 tangent = activeSpline.EvaluateTangent(t);
                if (tangent != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(tangent);
                    transform.rotation = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
                }
            }
        }

        public void Jump()
        {
            bool canJump = true;
            if (attachmentManager != null && attachmentManager.currentAttachedCount > 3) canJump = false;

            if (charController.isGrounded && canJump)
            {
                _verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private void ApplyGravity()
        {
            if (charController.isGrounded && _verticalVelocity.y < 0)
            {
                _verticalVelocity.y = -2f; 
            }

            _verticalVelocity.y += gravity * Time.deltaTime;
        }

        public void SwitchSpline(SplineContainer newSpline, float startDistance = 0f)
        {
            activeSpline = newSpline;
            _splineLength = activeSpline.CalculateLength();
            _currentDistance = startDistance;
        }
    }
}