using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics; // Indispensable pour les calculs de Spline
using PinePie.SimpleJoystick; // Ton joystick
using EnemyAttachmentSystem.Runtime;
using FeatherSystem.Runtime;

namespace Character.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private JoystickController joystick;
        public SplineContainer activeSpline;
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
        
        [Header("Gliding Settings")]
        [SerializeField] private float glideGravity = -2f; 
        private bool isGliding = false;

        [Header("State")]
        public bool isMovementLocked = false;
        public bool isHunted = false;

        public float _currentDistance = 0f;
        private float _splineLength;
        private float _currentSpeedValue;
        private Vector3 _verticalVelocity;
        
        public float VerticalInput { get; private set; } 
        public float SideInput { get; private set; }    
        public float CurrentDistance => _currentDistance;

        [Header("Respawn System")]
        private Vector3 _lastCheckpointPos;
        private SplineContainer _lastCheckpointSpline;
        private float _lastCheckpointDistance;

        void Awake()
        {
            charController = GetComponent<CharacterController>();
            attachmentManager = GetComponent<PlayerAttachmentManager>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            if (activeSpline != null)
            {
                _splineLength = activeSpline.CalculateLength();
                SetCheckpoint(transform.position, activeSpline, 0f);
            }
        }

        void Update()
        {
            if (isMovementLocked || activeSpline == null) return;
            
            if (charController.isGrounded && isGliding)
            {
                isGliding = false;
            }
            
            Vector2 input = (joystick != null) ? joystick.InputDirection : Vector2.zero;
            VerticalInput = input.y;

            ApplyMovementOnSpline(input);
        }

        private void ApplyMovementOnSpline(Vector2 input)
        {
            float t = _currentDistance / _splineLength;
            Vector3 tangent = activeSpline.EvaluateTangent(t);
            Vector2 splineDir = new Vector2(tangent.x, tangent.z).normalized;
            
            float combinedInput = Vector2.Dot(input, splineDir);
            
            Vector2 perpendicularDir = new Vector2(-splineDir.y, splineDir.x);
            SideInput = Vector2.Dot(input, perpendicularDir);
            
            float weightMultiplier = (attachmentManager != null) ? attachmentManager.currentSpeed / attachmentManager.baseSpeed : 1f;
            float mult = isHunted ? runMultiplier : 1f;
            float targetSpeed = combinedInput * speed * mult * weightMultiplier;

            _currentSpeedValue = Mathf.Lerp(_currentSpeedValue, targetSpeed, acceleration * Time.deltaTime);
            _currentDistance += _currentSpeedValue * Time.deltaTime;
            _currentDistance = Mathf.Clamp(_currentDistance, 0f, _splineLength);
            
            Vector3 targetSplinePos = activeSpline.EvaluatePosition(t);
            Vector3 horizontalMove = targetSplinePos - transform.position;
            horizontalMove.y = 0;
            
            ApplyGravity();
            
            charController.Move(horizontalMove + (_verticalVelocity * Time.deltaTime));
            
            if (Mathf.Abs(combinedInput) > 0.1f)
            {
                _spriteRenderer.flipX = (combinedInput < 0);
                
                if (tangent != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(tangent);
                    transform.rotation = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
                }
            }
        }

        public void Jump()
        {
            PlayerPowers powers = Object.FindFirstObjectByType<PlayerPowers>();
            
            if (charController.isGrounded)
            {
                _verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isGliding = false;
            }
            else if (powers.hasGlideFeather)
            {
                isGliding = !isGliding;
                
                if(isGliding && _verticalVelocity.y < 0) 
                {
                    _verticalVelocity.y = -1f; 
                }
            }
        }

        private void ApplyGravity()
        {
            if (charController.isGrounded && _verticalVelocity.y < 0)
            {
                _verticalVelocity.y = -2f;
                isGliding = false;
                return;
            }
            
            float currentGravity = isGliding ? glideGravity : gravity;
    
            _verticalVelocity.y += currentGravity * Time.deltaTime;
            
            if (isGliding && _verticalVelocity.y < glideGravity)
            {
                _verticalVelocity.y = glideGravity;
            }
        }

        public void SwitchSpline(SplineContainer newSpline)
        {
            if (newSpline == null || activeSpline == newSpline) return;
            
            charController.enabled = false;
            
            _verticalVelocity = Vector3.zero;
            _currentSpeedValue = 0f;
            
            activeSpline = newSpline;
            _splineLength = activeSpline.CalculateLength();
            if (_splineLength <= 0f) _splineLength = 0.1f;
            
            float3 localPlayerPos = newSpline.transform.InverseTransformPoint(transform.position);
            
            SplineUtility.GetNearestPoint(newSpline.Spline, localPlayerPos, out float3 nearestLocalPoint, out float t);
            
            _currentDistance = t * _splineLength;
            
            Vector3 exactWorldPos = newSpline.transform.TransformPoint(nearestLocalPoint);
            
            transform.position = exactWorldPos;
            charController.enabled = true;
    
            Debug.Log($"Switch 100% Automatique sur {newSpline.name} !");
        }

        public void SetCheckpoint(Vector3 pos, SplineContainer spline, float distance)
        {
            _lastCheckpointPos = pos;
            _lastCheckpointSpline = spline;
            _lastCheckpointDistance = distance;
        }

        public void Respawn()
        {
            _currentSpeedValue = 0;
            _verticalVelocity = Vector3.zero;
            
            activeSpline = _lastCheckpointSpline;
            _currentDistance = _lastCheckpointDistance;
            _splineLength = activeSpline.CalculateLength();

            charController.enabled = false;
            transform.position = _lastCheckpointPos;
            charController.enabled = true;
        }
    }
}