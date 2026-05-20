using UnityEngine;

namespace PuzzleSystem
{
    public class Crank : MonoBehaviour
    {
        [Header("Feedback - tremblement")]
        [SerializeField] private float shakeIntensity = 0.02f;
        [SerializeField] private float shakeSpeed     = 6f;

        private Vector3 _originPos;
        private bool    _blocked  = true;
        private bool    _shaking  = false;

        public bool IsBlocked => _blocked;
        

        private void Awake() => _originPos = transform.localPosition;

        private void Update()
        {
            if (_shaking && _blocked)
                transform.localPosition = _originPos + (Vector3)(UnityEngine.Random.insideUnitCircle * shakeIntensity)
                    * Mathf.Sin(Time.time * shakeSpeed);
        }
        

        public void StartShaking() => _shaking = true;

        public void Unblock()
        {
            _blocked = false;
            _shaking = false;
            transform.localPosition = _originPos;
        }
    }
}