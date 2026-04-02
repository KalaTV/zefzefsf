using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Gameplay.UI
{
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance;
        
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 1f;

        private void Awake()
        {
            Instance = this;
            canvasGroup.alpha = 0f;
        }

        public IEnumerator FadeOut()
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = timer / fadeDuration;
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        public IEnumerator FadeIn()
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = 1f - (timer / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }
    }
}