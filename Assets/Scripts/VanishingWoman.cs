using UnityEngine;

public class VanishingWoman : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject womanModel;
    [SerializeField] private AudioSource optionalScareSound;

    // Reset the model when the GameManager activates this anomaly again
    private void OnEnable()
    {
        if (womanModel != null)
        {
            womanModel.SetActive(true);
        }
    }

    // This triggers automatically when an object enters our Box Collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that stepped into the trigger is the Player
        if (other.CompareTag("Player"))
        {
            // Make sure the model is actually active before vanishing her
            if (womanModel != null && womanModel.activeSelf)
            {
                // Play a creepy sound effect right as she disappears (optional)
                if (optionalScareSound != null)
                {
                    optionalScareSound.Play();
                }

                // Make her vanish!
                womanModel.SetActive(false);
                Debug.Log("Player got too close! The woman vanished.");
            }
        }
    }
}