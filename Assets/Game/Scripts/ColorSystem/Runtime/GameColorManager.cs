using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement; // REQUIS : Pour pouvoir recharger la scène
using System.Collections;
using Character.Runtime;          // REQUIS : Pour bloquer le PlayerController
using Gameplay.UI;               // REQUIS : Pour le ScreenFader

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

        private bool isDead = false; // Sécurité pour éviter de mourir en boucle dans l'Update

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
            // Si le joueur est déjà mort, on arrête de vider la couleur
            if (isDead) return;

            if (colorAdjustments != null && currentSaturation > minSaturation)
            {
                currentSaturation -= decaySpeed * Time.deltaTime;
                currentSaturation = Mathf.Clamp(currentSaturation, minSaturation, maxSaturation);
                colorAdjustments.saturation.value = currentSaturation;

                // NOUVEAU : Si la saturation atteint le minimum, le joueur meurt
                if (currentSaturation <= minSaturation)
                {
                    StartColorDeath();
                }
            }
        }

        public void RestoreColor(float amount)
        {
            if (isDead) return; // Impossible de soigner un mort

            currentSaturation += amount;
            currentSaturation = Mathf.Clamp(currentSaturation, minSaturation, maxSaturation);
            colorAdjustments.saturation.value = currentSaturation;
        
            Debug.Log("Couleur récupérée !");
        }

        private void StartColorDeath()
        {
            isDead = true;
            
            // On cherche le joueur pour bloquer ses mouvements pendant qu'on meurt
            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            
            StartCoroutine(ColorDeathRoutine(player));
        }

        private IEnumerator ColorDeathRoutine(PlayerController player)
        {
            // 1. On fige le joueur si on l'a trouvé
            if (player != null)
            {
                player.isMovementLocked = true;
            }

            // 2. Transition vers l'écran noir (comme les DeathZones)
            if (ScreenFader.Instance != null)
                yield return ScreenFader.Instance.FadeOut();

            yield return new WaitForSeconds(0.5f); // Petite pause dramatique dans le noir

            // 3. ON RECOMMENCE LE JEU (Recharge la scène active actuelle de zéro)
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}