using System;
using UnityEngine;

namespace HideAndSeekSystem.Runtime
{
    public class ShadowMover : MonoBehaviour
    {
        [Header("Shadow Mover Settings")]
        [SerializeField] private float minSpeed = 4f;
        [SerializeField] private float maxSpeed = 8f;
        private float currentSpeed;
        [SerializeField] private float lifeTime = 4f;

        private void Start()
        {
            currentSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
            float scale = UnityEngine.Random.Range(0.8f, 1.5f);
            transform.localScale *= scale;
            
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
    }
}