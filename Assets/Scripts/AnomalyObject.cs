using UnityEngine;

public class AnomalyObject : MonoBehaviour
{
    // We register this object with the GameManager so it can be controlled
    void Start()
    {
        GameManager.Instance.RegisterAnomalyObject(this.gameObject);
    }

    // Function to hide or show the object
    public void SetAnomalyState(bool isAnomaly)
    {
        // If it's an anomaly, the object disappears (SetActive = false)
        // If it's normal, the object is visible (SetActive = true)
        gameObject.SetActive(!isAnomaly);
    }
}