using UnityEngine;
using System.Collections;

// Requirement: The GameObject must have a Light component for this script to work.
[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    [Header("References")]
    [Tooltip("If unassigned, the script will automatically get the Light component from this GameObject.")]
    [SerializeField] private Light targetLight; // The light component we will control

    [Header("Flicker Intensity Settings")]
    [Tooltip("The maximum intensity the light reaches when it is ON.")]
    [Range(0f, 5f)]
    [SerializeField] private float maxIntensity = 1.0f;

    [Tooltip("The depth of the standard flickering. 0 = no standard flicker, 1 = flickers near zero.")]
    [Range(0f, 1f)]
    [SerializeField] private float flickerDepth = 0.8f;

    [Header("Timing Settings")]
    [Tooltip("How fast the standard flicker changes (seconds). Lower values mean faster flicker.")]
    [Range(0.01f, 0.5f)]
    [SerializeField] private float flickerSpeed = 0.05f;

    [Header("Sparking Out Settings (Spooky Effect)")]
    [Tooltip("If enabled, the light will occasionally turn completely OFF (spark out).")]
    [SerializeField] private bool canSparkOut = true;

    [Tooltip("Percentage chance per flicker interval that the light will spark out (0.0 - 1.0).")]
    [Range(0f, 0.1f)]
    [SerializeField] private float sparkChance = 0.02f; // 2% chance per change

    [Tooltip("Minimum duration (seconds) the light stays dark when sparking out.")]
    [Range(0.1f, 2f)]
    [SerializeField] private float minSparkDuration = 0.1f;

    [Tooltip("Maximum duration (seconds) the light stays dark when sparking out.")]
    [Range(0.1f, 2f)]
    [SerializeField] private float maxSparkDuration = 0.5f;

    // Use to check for sparking state
    private bool isSparkingOut = false;

    void Start()
    {
        // Automatically get the Light component if not manually assigned in the Inspector
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }

        // Error check
        if (targetLight == null)
        {
            Debug.LogError($"[FlickeringLight] Missing Light component on {gameObject.name}!");
            enabled = false; // Disable script to prevent errors
            return;
        }

        // Start the automated flickering loop
        StartCoroutine(FlickerLoop());
    }

    // The main asynchronous loop that handles all timing
    IEnumerator FlickerLoop()
    {
        while (true) // Infinite loop as long as script is active
        {
            // --- Standard Spooky Flickering Logic ---
            // Calculate a random intensity lower than maxIntensity, based on flickerDepth
            float randomBase = maxIntensity - (flickerDepth * maxIntensity);
            float currentFlickerIntensity = Random.Range(randomBase, maxIntensity);

            targetLight.intensity = currentFlickerIntensity;

            // --- Sparking Out Logic (Random Dark Periods) ---
            if (canSparkOut && !isSparkingOut && Random.value < sparkChance)
            {
                // Begin sparking out (the light dies momentarily)
                StartCoroutine(SparkOutProcess());
            }

            // Wait for the next interval before changing intensity again
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    // Coroutine to handle the light going completely dark
    IEnumerator SparkOutProcess()
    {
        isSparkingOut = true;

        // Turn light completely off
        targetLight.intensity = 0.0f;

        // Pick random dark duration
        float duration = Random.Range(minSparkDuration, maxSparkDuration);
        yield return new WaitForSeconds(duration);

        isSparkingOut = false; // Coroutine finishes, main loop resumes normal flicker
    }
}