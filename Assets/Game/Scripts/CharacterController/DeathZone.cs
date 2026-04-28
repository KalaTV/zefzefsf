using UnityEngine;
using Character.Runtime;
using Gameplay.UI;
using System.Collections;

public class DeathZone : MonoBehaviour
{
    private bool isResetting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isResetting)
        {
            StartCoroutine(DeathRoutine(other.GetComponent<PlayerController>()));
        }
    }

    private IEnumerator DeathRoutine(PlayerController player)
    {
        isResetting = true;
        player.isMovementLocked = true;
        
        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeOut();
        
        player.Respawn();

        yield return new WaitForSeconds(0.2f);

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeIn();

        player.isMovementLocked = false;
        isResetting = false;
    }
}