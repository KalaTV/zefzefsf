using UnityEngine;

namespace CircleEyeSystem.Runtime
{
    /// <summary>
    /// À placer sur l'Iris (SpriteRenderer enfant du globe oculaire).
    /// L'iris se déplace dans le globe pour pointer vers le RandomMovingCircle.
    ///
    /// Hiérarchie Unity :
    ///   EyeGlobe   (SpriteRenderer globe, fixe dans la scène)
    ///   └── Iris   (SpriteRenderer iris + IrisController)  ← ce script
    /// </summary>
    public class IrisController : MonoBehaviour
    {
        [Header("Cible")]
        [Tooltip("Le Transform du RandomMovingCircle à regarder")]
        public Transform circleTarget;

        [Header("Globe")]
        [Tooltip("Rayon max en unités locales dans lequel l'iris peut bouger")]
        public float globeRadius = 0.25f;

        [Tooltip("Vitesse de suivi fluide")]
        public float followSpeed = 5f;

        [Header("Panique (joueur détecté)")]
        public float panicShakeAmount = 0.04f;
        public float panicShakeSpeed  = 30f;
        public float normalScale      = 1f;
        public float panicScale       = 1.35f;
        public float scaleSpeed       = 6f;

        // ─────────────────────────────────────────
        private Transform globeTransform; // parent = globe oculaire
        private Vector3   currentLocalPos;
        private bool      isPanicking = false;
        private float     targetScale;
        private float     panicTimer  = 0f;

        private void Awake()
        {
            globeTransform = transform.parent;
            targetScale    = normalScale;

            if (globeTransform == null)
            {
                Debug.LogError("[IrisController] L'iris doit être enfant du globe oculaire !", this);
                enabled = false;
            }
        }

        private void Update()
        {
            Vector3 target = ComputeTargetLocalPos();

            if (isPanicking)
            {
                panicTimer += Time.deltaTime;
                target     += ComputeShakeOffset();
            }

            // Lerp fluide
            currentLocalPos = Vector3.Lerp(
                currentLocalPos, target, followSpeed * Time.deltaTime
            );

            // Clamp dans le globe
            Vector3 clamped = Vector3.ClampMagnitude(currentLocalPos, globeRadius);
            transform.localPosition = new Vector3(clamped.x, clamped.y, -0.01f);

            // Scale
            float s = Mathf.Lerp(transform.localScale.x, targetScale, scaleSpeed * Time.deltaTime);
            transform.localScale = Vector3.one * s;
        }

        private Vector3 ComputeTargetLocalPos()
        {
            if (circleTarget == null) return Vector3.zero;

            // Vecteur monde du globe vers le cercle
            Vector3 delta = circleTarget.position - globeTransform.position;

            // Caméra regarde vers Z → Z est l'axe visible en profondeur qu'on ignore.
            // On ne garde que X (horizontal) et Y (vertical) pour la direction 2D.
            delta.z = 0f;

            if (delta.sqrMagnitude < 0.001f) return Vector3.zero;

            // Normaliser APRÈS aplatissement pour éviter que la grande
            // différence de hauteur (globe en l'air, cercle au sol) écrase
            // la composante horizontale.
            Vector3 dir2D = delta.normalized;

            // Passer en espace local du globe
            Vector3 localDir = globeTransform.InverseTransformDirection(dir2D);
            localDir.z = 0f;

            if (localDir.sqrMagnitude < 0.001f) return Vector3.zero;

            return localDir.normalized * globeRadius;
        }

        private Vector3 ComputeShakeOffset()
        {
            float nx = (Mathf.PerlinNoise(panicTimer * panicShakeSpeed, 0f)       - 0.5f) * 2f;
            float ny = (Mathf.PerlinNoise(0f, panicTimer * panicShakeSpeed + 99f) - 0.5f) * 2f;
            return new Vector3(nx, ny, 0f) * panicShakeAmount;
        }

        // ── API appelée par RandomMovingCircle ──────────────────────
        public void OnPlayerDetected()
        {
            if (isPanicking) return;
            isPanicking = true;
            panicTimer  = 0f;
            targetScale = panicScale;
        }

        public void OnPlayerLost()
        {
            isPanicking = false;
            targetScale = normalScale;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (transform.parent == null) return;
            Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
            Gizmos.DrawWireSphere(transform.parent.position, globeRadius);
            if (circleTarget != null)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.7f);
                Gizmos.DrawLine(transform.parent.position, circleTarget.position);
            }
        }
#endif
    }
}