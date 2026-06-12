using UnityEngine;

public class Anomaly : MonoBehaviour
{
    public enum AnomalyType { Simple, Difficult, Disturbing }
    // Defines how this anomaly behaves when active
    public enum AnomalyBehavior { HideWhenAnomalyPresent, ShowWhenAnomalyPresent }

    [Header("GDD Kategorie")]
    public AnomalyType type;
    public AnomalyBehavior behavior;

    [Header("Beschreibung")]
    public string description;

    // Auto-register with the GameManager when the game starts
    protected virtual void Awake()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterAnomalyObject(this.gameObject);
        }
    }
}