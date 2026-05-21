using System;
using UnityEngine;

namespace PuzzleSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class Debris : MonoBehaviour
    {
        public event Action<Debris> OnCleared;

        private Vector3    _originPos;
        private bool       _isCleared = false;
        private bool       _shaking   = false;
        private Rigidbody  _rb;
        private Vector3 _originWorldPos;
        private float _pushThreshold = 0.5f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;        
            _originPos = transform.localPosition;
            _originWorldPos = transform.position;
        }

        private void Update()
        {
            if (!_isCleared && !_rb.isKinematic)
            {
                float moved = Vector3.Distance(transform.position, _originWorldPos);
                if (moved >= _pushThreshold)
                    Clear();
            }
        }
        
        
        
        private void OnTriggerExit(Collider other)
        {
            if (_isCleared) return;
            if (!other.CompareTag("Player")) return;

            // On libère juste la physique, pas encore Clear()
            _rb.isKinematic = false;
        }
        
        public void Clear()
        {
            if (_isCleared) return;
            _isCleared = true;
            
            _rb.isKinematic = false;          

            OnCleared?.Invoke(this);
        }
    }
}