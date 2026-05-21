using System.Collections;
using UnityEngine;

namespace PuzzleSystem
{
    public class FaceRotation : MonoBehaviour
    {
        [Header("Les deux parties")]
        [SerializeField] private Transform faceLeft;
        [SerializeField] private Transform faceRight;

        [Header("Animation ouverture")]
        [SerializeField] private float openDistance = 1.5f; // distance d'écartement
        [SerializeField] private float openDuration = 1.2f;
        [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector3 _originPos;
        private bool    _shaking = false;

        private void Awake() => _originPos = transform.localPosition;
        

        public IEnumerator PlayOpenAnimation()
        {
            Vector3 leftStart  = faceLeft.localPosition;
            Vector3 rightStart = faceRight.localPosition;

            // La gauche part à gauche, la droite part à droite
            Vector3 leftEnd  = leftStart  + Vector3.left  * openDistance;
            Vector3 rightEnd = rightStart + Vector3.right * openDistance;

            float elapsed = 0f;
            while (elapsed < openDuration)
            {
                elapsed += Time.deltaTime;
                float t = openCurve.Evaluate(elapsed / openDuration);

                faceLeft.localPosition  = Vector3.Lerp(leftStart,  leftEnd,  t);
                faceRight.localPosition = Vector3.Lerp(rightStart, rightEnd, t);

                yield return null;
            }

            faceLeft.localPosition  = leftEnd;
            faceRight.localPosition = rightEnd;
        }
    }
}