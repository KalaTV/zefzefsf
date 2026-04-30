using UnityEngine;
using Character.Runtime;
public class WindTrigger : MonoBehaviour
{
    public float force = 5f;
    public float duration = 5f;
    public ParticleSystem windParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TriggerWind(force, duration);
                if (windParticles != null) windParticles.Play();
            }
        }
    }
}