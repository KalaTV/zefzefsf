using System;
using UnityEngine;

namespace PuzzleSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class Debris : MonoBehaviour
    {
        [Header("Feedback - tremblement")]
        [SerializeField] private float shakeIntensity = 0.03f;
        [SerializeField] private float shakeSpeed     = 8f;

        public event Action<Debris> OnCleared;

        private Vector3    _originPos;
        private bool       _isCleared = false;
        private bool       _shaking   = false;
        private Rigidbody  _rb;
        

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;        
            _originPos = transform.localPosition;
        }

        private void Update()
        {
            if (_shaking && !_isCleared)
                transform.localPosition = _originPos + (Vector3)(UnityEngine.Random.insideUnitCircle * shakeIntensity)
                                                     * Mathf.Sin(Time.time * shakeSpeed);
        }
        

        public void StartShaking() => _shaking = true;
        public void StopShaking()
        {
            _shaking = false;
            transform.localPosition = _originPos;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (_isCleared) return;
            if (!other.CompareTag("Player")) return;

            Clear();
        }
        
        public void Clear()
        {
            if (_isCleared) return;
            _isCleared = true;

            StopShaking();
            _rb.isKinematic = false;          

            OnCleared?.Invoke(this);
        }
    }
}