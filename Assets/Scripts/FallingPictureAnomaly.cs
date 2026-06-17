using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPictureAnomaly : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Drag a GameObject with a BoxCollider (set as Trigger) that will detect the player walking by.")]
    [SerializeField] private Collider triggerCollider;

    [Header("Physics Settings")]
    [Tooltip("Slight force to push the picture off the wall when it falls.")]
    [SerializeField] private float pushForce = 0.5f;
    [Tooltip("Increases how fast the picture falls (e.g. 2.0 = double gravity).")]
    [SerializeField] private float gravityMultiplier = 2.5f;

    [Header("Assign the original, clean Picture from the scene")]
    [SerializeField] private GameObject normalPictureObject;

    [Header("Audio Settings")]
    [Tooltip("AudioSource to play the crash sound.")]
    [SerializeField] private AudioSource impactAudioSource;
    [Tooltip("Sound clip of the picture hitting the ground.")]
    [SerializeField] private AudioClip impactSound;
    [Tooltip("Minimum velocity required to trigger the sound (prevents noise from tiny slides).")]
    [SerializeField] private float minImpactForce = 0.2f;
    [Tooltip("Cooldown delay (in seconds) after falling starts before the sound can be played. Prevents wall collision sound.")]
    [SerializeField] private float soundDelayCooldown = 0.25f;

    private Rigidbody rb;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool hasFallen = false;
    private bool hasHitGround = false;
    private float soundAllowedTime = 0f;
    private bool hasHitGroundChecked = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        // Ensure physics is locked initially so it doesn't fall on its own
        rb.isKinematic = true;
    }

    private void OnEnable()
    {
        // Hide the normal picture when the anomaly begins
        if (normalPictureObject != null)
        {
            normalPictureObject.SetActive(false);
        }

        // Reset state when the anomaly is activated by the GameManager
        ResetPicture();
    }

    private void OnDisable()
    {
        // Bring back the normal picture when the floor resets
        if (normalPictureObject != null)
        {
            normalPictureObject.SetActive(true);
        }

        // Reset state when the floor resets
        ResetPicture();
    }

    private void FixedUpdate()
    {
        // Apply extra downward gravity while falling to speed it up
        if (hasFallen && !hasHitGround && rb != null && !rb.isKinematic)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Fall only once when the player steps into the trigger
        if (!hasFallen && other.CompareTag("Player"))
        {
            TriggerFall();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Play the crash sound only after the cooldown has passed (ignoring the initial wall detach collision)
        if (hasFallen && !hasHitGround && Time.time >= soundAllowedTime)
        {
            if (collision.relativeVelocity.magnitude > minImpactForce)
            {
                hasHitGround = true;
                if (impactAudioSource != null && impactSound != null)
                {
                    impactAudioSource.PlayOneShot(impactSound);
                }
            }
        }
    }

    private void TriggerFall()
    {
        hasFallen = true;
        rb.isKinematic = false;
        
        // Set the timestamp when the sound is allowed to trigger (ignoring the first moments)
        soundAllowedTime = Time.time + soundDelayCooldown;

        // Optional: add a tiny nudge force forward so it rolls off the wall nail realistically
        rb.AddForce(-transform.forward * pushForce, ForceMode.Impulse);
        rb.AddTorque(transform.right * pushForce, ForceMode.Impulse);

        Debug.Log($"[FallingPictureAnomaly] {gameObject.name} fell off the wall!");
    }

    private void ResetPicture()
    {
        hasFallen = false;
        hasHitGround = false;
        soundAllowedTime = 0f;
 
        // Stop any current velocity
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Return to original hanging position
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}