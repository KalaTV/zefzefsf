using UnityEngine;
using Unity.Mathematics;
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
        [SerializeField] public SplineContainer activeSpline;
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

        [Header("Respawn System")]
        private Vector3 _lastCheckpointPos;
        private SplineContainer _lastCheckpointSpline;
        private float _lastCheckpointDistance;

        public float _currentDistance = 0f;
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
            float weightMultiplier = (attachmentManager != null) ? attachmentManager.currentSpeed / attachmentManager.baseSpeed : 1f;
            float mult = isHunted ? runMultiplier : 1f;
            float targetSpeed = inputX * speed * mult * weightMultiplier;

            _currentSpeedValue = Mathf.Lerp(_currentSpeedValue, targetSpeed, acceleration * Time.deltaTime);
            _currentDistance += _currentSpeedValue * Time.deltaTime;
            _currentDistance = Mathf.Clamp(_currentDistance, 0f, _splineLength);
            
            float t = _currentDistance / _splineLength;
            Vector3 targetSplinePos = activeSpline.EvaluatePosition(t);
            Vector3 horizontalMove = targetSplinePos - transform.position;
            horizontalMove.y = 0; 
            
            ApplyGravity();
            
            charController.Move(horizontalMove + (_verticalVelocity * Time.deltaTime));
            
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

        public void SwitchSpline(SplineContainer newSpline)
        {
            if (newSpline == null) return;

            activeSpline = newSpline;
            _splineLength = activeSpline.CalculateLength();
            var splineData = activeSpline.Spline;

            // 1. TRADUCTION : On donne notre position "Monde" et on la convertit en position "Locale" pour la spline
            float3 localPlayerPos = activeSpline.transform.InverseTransformPoint(transform.position);
    
            // 2. CALCUL : La spline cherche le point le plus proche dans son espace à elle
            SplineUtility.GetNearestPoint(splineData, localPlayerPos, out float3 nearestLocalPos, out float nearestT);
    
            // 3. RETRADUCTION : On reconvertit ce point local en vraies coordonnées 3D du jeu
            Vector3 nearestWorldPos = activeSpline.transform.TransformPoint(nearestLocalPos);

            // 4. On met à jour la distance
            _currentDistance = nearestT * _splineLength;

            // 5. On déplace le perso à la bonne place (en gardant son Y pour la gravité)
            transform.position = new Vector3(nearestWorldPos.x, transform.position.y, nearestWorldPos.z);
    
            Debug.Log($"Switch validé sur {newSpline.name} à {_currentDistance:F1}m");
        }
        public void SetCheckpoint(Vector3 pos, SplineContainer spline, float distance)
        {
            _lastCheckpointPos = pos;
            _lastCheckpointSpline = spline;
            _lastCheckpointDistance = distance;
            Debug.Log("Checkpoint sauvegardé !");
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
    
            Debug.Log("Respawn au dernier checkpoint !");
        }
    }
}