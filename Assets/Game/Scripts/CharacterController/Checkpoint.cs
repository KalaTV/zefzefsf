using UnityEngine;
using Character.Runtime;
using UnityEngine.Splines;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            
            player.SetCheckpoint(transform.position, player.activeSpline, player.currentDistance);
        }
    }
}