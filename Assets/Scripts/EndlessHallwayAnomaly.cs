using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EndlessHallwayAnomaly : MonoBehaviour
{
    [Header("Loop Settings")]
    [Tooltip("How many meters backward should we teleport the player? (Should match your hallway's tile pattern, e.g. 10 or 12 meters)")]
    public float loopDistance = 12f;

    [Header("Lights Settings")]
    [Tooltip("The duration the lights stay off during the teleport (blackout time).")]
    [SerializeField] private float hold = 0.15f;
    [Tooltip("These lights will turn off during the teleportation step (optional).")]
    [SerializeField] private Light[] flickerLights;

    private Transform playerTransform;
    private CharacterController playerController;
    private bool isTransitioning = false;

    private void Start()
    {
        // Automatically make sure the collider is set up as a trigger
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Cache references
        if (GameManager.Instance != null)
        {
            playerTransform = GameManager.Instance.player;
            if (playerTransform != null)
            {
                playerController = playerTransform.GetComponent<CharacterController>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player stepped into the trigger
        if (!isTransitioning && other.CompareTag("Player") && playerTransform != null)
        {
            StartCoroutine(LoopPlayerCoroutine());
        }
    }

    private IEnumerator LoopPlayerCoroutine()
    {
        isTransitioning = true;

        // 1. Turn off the lights
        SetLightsState(false);

        // 2. Teleport the player
        if (playerController != null) playerController.enabled = false;

        playerTransform.position = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y,
            playerTransform.position.z - loopDistance
        );

        if (playerController != null) playerController.enabled = true;

        // 3. Hold in darkness for a brief moment
        yield return new WaitForSeconds(hold);

        // 4. Turn the lights back on
        SetLightsState(true);

        isTransitioning = false;
        Debug.Log("[EndlessHallwayAnomaly] Player successfully looped and lights flickered!");
    }

    private void SetLightsState(bool state)
    {
        if (flickerLights == null) return;
        
        foreach (Light lightSource in flickerLights)
        {
            if (lightSource != null)
            {
                lightSource.enabled = state;
            }
        }
    }
}