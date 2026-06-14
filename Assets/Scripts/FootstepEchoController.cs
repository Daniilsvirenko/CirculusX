using System.Collections;
using UnityEngine;

public class FootstepEchoController : MonoBehaviour
{
    [Header("References")]
    public AudioSource echoAudioSource;
    public AudioClip footstepSound;
    [Tooltip("Assign your main Player object here so the script knows where 'behind' is.")]
    public Transform playerTransform;

    [Header("Settings")]
    public float echoDelay = 0.35f;
    [Tooltip("How many meters behind the player should the ghost step appear?")]
    public float distanceBehindPlayer = 2.0f;

    /// <summary>
    /// Call this from PlayerMovement every time the player takes a step.
    /// </summary>
    public void RegisterPlayerStep()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is missing on FootstepEchoController!");
            return;
        }

        // Calculate the exact position behind the player RIGHT NOW
        // playerTransform.forward points ahead, so multiplying by negative distance points backward
        Vector3 spawnPosition = playerTransform.position - (playerTransform.forward * distanceBehindPlayer);

        // Ensure it stays on the ground level
        spawnPosition.y = playerTransform.position.y;

        // Start the delayed audio routine, passing the calculated world position
        StartCoroutine(PlayEchoStepAfterDelay(spawnPosition));
    }

    private IEnumerator PlayEchoStepAfterDelay(Vector3 worldPosition)
    {
        // Wait out of tact with the player
        yield return new WaitForSeconds(echoDelay);

        if (echoAudioSource != null && footstepSound != null)
        {
            // Move this Anomaly GameObject to the physical position behind where the player WAS
            transform.position = worldPosition;

            // Play the sound spatially at this new position
            echoAudioSource.PlayOneShot(footstepSound);
        }
    }
}