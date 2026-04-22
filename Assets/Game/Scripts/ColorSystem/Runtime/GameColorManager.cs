using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace ColorSystem.Runtime
{
    public class GameColorManager : MonoBehaviour
    {
        [SerializeField] private Volume globalVolume;
        private ColorAdjustments colorAdjustments;
        
        [Header("Saturation Settings")]
        [SerializeField] private float decaySpeed = 0.1f;

        public float maxSaturation { get; private set; }
        [SerializeField] private float minSaturation = -100f;
        
        public float currentSaturation = 0f;
        void Start()
        {
            maxSaturation = 0f;
            if (globalVolume.profile.TryGet(out colorAdjustments))
            {
                currentSaturation = maxSaturation;
                colorAdjustments.saturation.value = currentSaturation;
            }
        }

        void Update()
        {
            if (colorAdjustments != null && currentSaturation > minSaturation)
            {
                currentSaturation -= decaySpeed * Time.deltaTime;
                
                currentSaturation = Mathf.Clamp(currentSaturation, minSaturation, maxSaturation);
            
                colorAdjustments.saturation.value = currentSaturation;
            }
            
        }

        
        public void RestoreColor(float amount)
        {
            currentSaturation += amount;
            currentSaturation = Mathf.Clamp(currentSaturation, minSaturation, maxSaturation);
            colorAdjustments.saturation.value = currentSaturation;
        
            Debug.Log("Couleur récupérée !");
        }
    }
    
}