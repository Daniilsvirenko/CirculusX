using UnityEngine;

public class PaintingAnomalyController : MonoBehaviour
{
    [Header("Assign the standalone Normal Picture from the scene")]
    public GameObject normalPictureGroup;

    // This runs the exact frame the GameManager turns this Anomaly Object ON
    private void OnEnable()
    {
        if (normalPictureGroup != null)
        {
            normalPictureGroup.SetActive(false); // Hide the normal picture
        }
    }

    // This runs the exact frame the GameManager resets the floor and turns this OFF
    private void OnDisable()
    {
        if (normalPictureGroup != null)
        {
            normalPictureGroup.SetActive(true);  // Bring back the normal picture
        }
    }
}