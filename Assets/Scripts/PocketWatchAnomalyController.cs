using UnityEngine;

public class PocketWatchAnomalyController : MonoBehaviour
{
    [Header("Assign the standalone Normal Watch parts/group from the scene")]
    public GameObject normalWatchGroup;

    // This runs the exact frame the GameManager turns this Anomaly Object ON
    private void OnEnable()
    {
        if (normalWatchGroup != null)
        {
            normalWatchGroup.SetActive(false); // Hide the normal watch
        }
    }

    // This runs the exact frame the GameManager resets the floor and turns this OFF
    private void OnDisable()
    {
        if (normalWatchGroup != null)
        {
            normalWatchGroup.SetActive(true);  // Bring back the normal watch
        }
    }
}