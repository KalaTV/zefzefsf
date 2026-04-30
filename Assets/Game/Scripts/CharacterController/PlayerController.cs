using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics; // Indispensable pour les calculs de Spline
using PinePie.SimpleJoystick; // Ton joystick
using EnemyAttachmentSystem.Runtime;
using FeatherSystem.Runtime;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Character.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private JoystickController joystick;
        public SplineContainer activeSpline;
        private PlayerAttachmentManager attachmentManager;
        private SpriteRenderer spriteRenderer;
        private CharacterController charController;

        [Header("Movement Settings")]
        public float speed = 5f;
        public float acceleration = 12f;
        public float runMultiplier = 2.18f;
        
        [Header("Fluidity Settings")]
        public float splineSwitchSpeed = 8f; 
        public float rotationSpeed = 12f;
        
        private Vector3 transitionOffset = Vector3.zero;
        
        [Header("Jump Settings")]
        public float jumpHeight = 1.2f;
        public float gravity = -15f;
        
        [Header("Gliding Settings")]
        [SerializeField] private float glideGravity = -2f; 
        private bool isGliding = false;

        [Header("Wind Settings")]
        private float windForce = 0f;
        private bool isWindActive = false;
        
        [Header("State")]
        public bool isMovementLocked = false;
        public bool isHunted = false;
        
        public float currentDistance = 0f;
        private float splineLength;
        private float currentSpeedValue;
        private Vector3 verticalVelocity;
        
        public float VerticalInput { get; private set; } 
        public float SideInput { get; private set; }    
        public float CurrentDistance => currentDistance;

        [Header("Respawn System")]
        private Vector3 lastCheckpointPos;
        private SplineContainer lastCheckpointSpline;
        private float lastCheckpointDistance;

        void Awake()
        {
            charController = GetComponent<CharacterController>();
            attachmentManager = GetComponent<PlayerAttachmentManager>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            if (activeSpline != null)
            {
                splineLength = activeSpline.CalculateLength();
                SetCheckpoint(transform.position, activeSpline, 0f);
            }
        }

        private void Start()
        {
            if (activeSpline != null)
            {
                splineLength = activeSpline.CalculateLength();
                if (splineLength <= 0) splineLength = 0.1f;
                
                float3 localPos = activeSpline.transform.InverseTransformPoint(transform.position);
                SplineUtility.GetNearestPoint(activeSpline.Spline, localPos, out float3 nearestLocal, out float t);
                
                currentDistance = t * splineLength;
                
                transitionOffset = Vector3.zero;
                
                Vector3 worldTarget = activeSpline.transform.TransformPoint(nearestLocal);
        
                charController.enabled = false;
                transform.position = worldTarget;
                charController.enabled = true;
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
            if (activeSpline == null || splineLength <= 0.1f) return;
            
            float t = currentDistance / splineLength;
            
            Vector3 targetSplineWorldPos = activeSpline.EvaluatePosition(t); 
            Vector3 tangent = activeSpline.EvaluateTangent(t);
            
            Vector2 splineDir = new Vector2(tangent.x, tangent.z).normalized;
            float combinedInput = Vector2.Dot(input, splineDir);
            
            Vector2 perpendicularDir = new Vector2(-splineDir.y, splineDir.x);
            SideInput = Vector2.Dot(input, perpendicularDir);
            
            float weightMultiplier = (attachmentManager != null) ? attachmentManager.currentSpeed / attachmentManager.baseSpeed : 1f;
            float mult = isHunted ? runMultiplier : 1f;
            float targetSpeed = combinedInput * speed * mult * weightMultiplier;

            currentSpeedValue = Mathf.Lerp(currentSpeedValue, targetSpeed, acceleration * Time.deltaTime);
            
            float finalMove = (currentSpeedValue + windForce) * Time.deltaTime;
            currentDistance += finalMove;
            
            currentDistance = Mathf.Clamp(currentDistance, 0f, splineLength);
            
            transitionOffset = Vector3.Lerp(transitionOffset, Vector3.zero, splineSwitchSpeed * Time.deltaTime);
            
            Vector3 finalTargetPos = targetSplineWorldPos + transitionOffset;
            
            Vector3 horizontalMove = finalTargetPos - transform.position;
            horizontalMove.y = 0; 

            ApplyGravity();
            
            charController.Move(horizontalMove + (verticalVelocity * Time.deltaTime));
            if (Mathf.Abs(combinedInput) > 0.1f)
            {
                if(spriteRenderer != null) spriteRenderer.flipX = (combinedInput < -0f);
            }
        }

        public void Jump()
        {
            PlayerPowers powers = Object.FindFirstObjectByType<PlayerPowers>();
                    
            if (charController.isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isGliding = false;
            }
            else if (powers.hasGlideFeather)
            {
                isGliding = !isGliding;
                        
                if(isGliding && verticalVelocity.y < 0) 
                {
                    verticalVelocity.y = -1f; 
                }
            }
        }

        private void ApplyGravity()
        {
            if (charController.isGrounded && verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
                isGliding = false;
                return;
            }
            
            float currentGravity = isGliding ? glideGravity : gravity;
    
            verticalVelocity.y += currentGravity * Time.deltaTime;
            
            if (isGliding && verticalVelocity.y < glideGravity)
            {
                verticalVelocity.y = glideGravity;
            }
        }

        public void SwitchSpline(SplineContainer newSpline)
        {
            if (newSpline == null || activeSpline == newSpline) return;
            
            Vector3 startPos = transform.position;
            
            activeSpline = newSpline;
            splineLength = activeSpline.CalculateLength();
            if (splineLength <= 0f) splineLength = 0.1f;
    
            float3 localPlayerPos = newSpline.transform.InverseTransformPoint(startPos);
            SplineUtility.GetNearestPoint(newSpline.Spline, localPlayerPos, out float3 nearestLocalPoint, out float t);
            currentDistance = t * splineLength;
    
            Vector3 exactWorldPos = newSpline.transform.TransformPoint(nearestLocalPoint);
            
            transitionOffset = startPos - exactWorldPos;
            transitionOffset.y = 0;
    
            
            Debug.Log("Switch Fluide sur {newSpline.name}");
        }

        public void SetCheckpoint(Vector3 pos, SplineContainer spline, float distance)
        {
            lastCheckpointPos = pos;
            lastCheckpointSpline = spline;
            lastCheckpointDistance = distance;
        }

        public void Respawn()
        {
            currentSpeedValue = 0;
            verticalVelocity = Vector3.zero;
            
            activeSpline = lastCheckpointSpline;
            currentDistance = lastCheckpointDistance;
            splineLength = activeSpline.CalculateLength();

            charController.enabled = false;
            transform.position = lastCheckpointPos;
            charController.enabled = true;
        }
        
        public void TriggerWind(float strength, float duration)
        {
            StartCoroutine(WindCoroutine(strength, duration));
        }
        
        private IEnumerator WindCoroutine(float strength, float duration)
        {
            isWindActive = true;
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                windForce = -strength; 
                yield return null;
            }
            windForce = 0f;
            isWindActive = false;
        }
    }
}