using UnityEngine;

public class SoundAnomaly : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;

    private bool hasPlayed = false;

    private void Awake()
    {
        // Fallback to grab the attached AudioSource if forgot to assign in inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Runs automatically whenever this GameObject is activated by the GameManager
    private void OnEnable()
    {
        hasPlayed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger zone is the Player
        if (other.CompareTag("Player") && !hasPlayed)
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
                hasPlayed = true; // Prevents the sound from spamming if they step in/out
                Debug.Log("[Sound Anomaly] Player triggered the door knocking audio.");
            }
        }
    }
}