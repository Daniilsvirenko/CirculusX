using UnityEngine;

// Forces an Anomaly component to exist alongside this script for your GameManager
[RequireComponent(typeof(Anomaly))]
public class ExitSignAnomalyController : MonoBehaviour
{
    [Header("Assign the original, clean Emergency Sign from the scene")]
    public GameObject normalSignObject;

    // This runs the exact frame the GameManager turns this Anomaly Object ON
    private void OnEnable()
    {
        if (normalSignObject != null)
        {
            normalSignObject.SetActive(false); // Hide the normal sign
        }
    }

    // This runs the exact frame the GameManager resets the floor and turns this OFF
    private void OnDisable()
    {
        if (normalSignObject != null)
        {
            normalSignObject.SetActive(true);  // Bring back the normal sign
        }
    }
}