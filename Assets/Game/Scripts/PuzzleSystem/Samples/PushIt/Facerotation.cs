using System.Collections;
using UnityEngine;

namespace PuzzleSystem
{
    public class FaceRotation : MonoBehaviour
    {
        [Header("Animation ouverture")]
        [SerializeField] private float openAngle    = 90f; 
        [SerializeField] private float openDuration = 1.2f;  
        [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Feedback - tremblement")]
        [SerializeField] private float shakeIntensity = 0.025f;
        [SerializeField] private float shakeSpeed     = 7f;

        private Vector3 _originPos;
        private bool    _shaking = false;
        

        private void Awake() => _originPos = transform.localPosition;

        private void Update()
        {
            if (_shaking)
                transform.localPosition = _originPos + (Vector3)(UnityEngine.Random.insideUnitCircle * shakeIntensity)
                                                     * Mathf.Sin(Time.time * shakeSpeed);
        }
        

        public void SetShaking(bool state)
        {
            _shaking = state;
            if (!state) transform.localPosition = _originPos;
        }
        
        public IEnumerator PlayOpenAnimation()
        {
            Quaternion startRot = transform.localRotation;
            Quaternion endRot   = startRot * Quaternion.Euler(0f, openAngle, 0f);

            float elapsed = 0f;
            while (elapsed < openDuration)
            {
                elapsed += Time.deltaTime;
                float t = openCurve.Evaluate(elapsed / openDuration);
                transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
                yield return null;
            }

            transform.localRotation = endRot;
        }
    }
}