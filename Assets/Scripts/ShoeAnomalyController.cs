using UnityEngine;

public class ShoeAnomalyController : MonoBehaviour
{
    [Header("Assign the standalone Normal Group from the scene")]
    public GameObject normalShoeGroup;

    // This runs the exact frame the GameManager turns this Anomaly Object ON
    private void OnEnable()
    {
        if (normalShoeGroup != null) normalShoeGroup.SetActive(false); // Hide the normal ones
    }

    // This runs the exact frame the GameManager resets the floor and turns this OFF
    private void OnDisable()
    {
        if (normalShoeGroup != null) normalShoeGroup.SetActive(true);  // Bring back the normal ones
    }
}

