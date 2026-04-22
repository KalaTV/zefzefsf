using UnityEngine;

namespace FeatherSystem.Runtime.Interactables
{
    public class BurnableObject : MonoBehaviour
    {
        [SerializeField] private float burnSpeed = 1f;
        
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
            if (isPlayerInside && !isBurning)
            {
                Debug.Log("Ignited the 3D item");
                isBurning = true;
            }
        }

        private void Update()
        {
            if (isBurning && meshRenderer != null)
            {
                burnProgress += burnSpeed * Time.deltaTime;
                
                Color targetColor = Color.Lerp(originalColor, Color.black, burnProgress);
                
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
            }
        }
    }
}