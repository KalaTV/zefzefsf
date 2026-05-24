using UnityEngine;
using System.Collections; // Requis pour utiliser les Coroutines

namespace FeatherSystem.Runtime.Interactables
{
    public class BurnableObject : MonoBehaviour
    {
        [Header("Burn Settings")]
        [SerializeField] private float burnSpeed = 1f;
        
        [Header("VFX Settings")]
        [Tooltip("Le prefab du projectile (ex: particule de feu)")]
        [SerializeField] private GameObject fireVfxPrefab;
        [Tooltip("Temps en secondes que met le VFX pour atteindre l'objet")]
        [SerializeField] private float vfxTravelDuration = 0.3f; 
        
        private GameObject player;
        private bool isPlayerInside = false;
        private bool isBurning = false;
        private MeshRenderer meshRenderer;
        private Color originalColor;
        private float burnProgress = 0f;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                originalColor = meshRenderer.material.color;
            }
        }

        public void OnInteract()
        {
            // On s'assure que le joueur est là, que ce n'est pas déjà en feu, 
            // et que la coroutine n'a pas déjà été lancée.
            if (isPlayerInside && !isBurning)
            {
                if (fireVfxPrefab != null && player != null)
                {
                    Debug.Log("Lancement du VFX vers l'objet...");
                    StartCoroutine(SendVfxRoutine());
                }
                else
                {
                    // Sécurité : S'il n'y a pas de VFX assigné, ça brûle instantanément
                    Debug.Log("Ignited the 3D item (No VFX setup)");
                    isBurning = true;
                }
            }
        }

        private IEnumerator SendVfxRoutine()
        {
            // 1. Instancier le VFX à la position du joueur
            // Optionnel : Tu peux ajouter un offset si tu veux que ça parte des mains du joueur
            // ex: player.transform.position + Vector3.up
            GameObject vfxInstance = Instantiate(fireVfxPrefab, player.transform.position, Quaternion.identity);
            
            Vector3 startPos = player.transform.position;
            Vector3 targetPos = transform.position; // Le centre du BurnableObject
            
            float elapsedTime = 0f;
            
            // 2. Déplacer le VFX vers la cible au fil du temps
            while (elapsedTime < vfxTravelDuration)
            {
                if (vfxInstance != null)
                {
                    // Lerp permet un mouvement fluide du point A au point B
                    vfxInstance.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / vfxTravelDuration);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null; // Attendre la frame suivante
            }
            
            // 3. L'impact a lieu
            if (vfxInstance != null)
            {
                vfxInstance.transform.position = targetPos;
                // On détruit le projectile après un court délai pour laisser les particules mourir naturellement
                Destroy(vfxInstance, 0.1f); 
            }

            // 4. On déclenche la combustion de l'objet
            Debug.Log("VFX a touché la cible, l'objet s'enflamme !");
            isBurning = true;
        }

        private void Update()
        {
            if (isBurning && meshRenderer != null)
            {
                burnProgress += burnSpeed * Time.deltaTime;
                
                Color targetColor = Color.Lerp(originalColor, Color.black, burnProgress);
                
                // Note: Pour que l'alpha (transparence) fonctionne, le Material de ton objet 
                // doit être configuré sur "Transparent" (ou "Fade") dans Unity.
                targetColor.a = 1f - burnProgress;
                
                meshRenderer.material.color = targetColor;

                if (burnProgress >= 1f)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                player = other.gameObject;
                isPlayerInside = true;
            }
            else if (other.CompareTag("Fire"))
            {
                isBurning = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInside = false;
                // Optionnel : player = null; si on veut purger la référence
            }
        }
    }
}