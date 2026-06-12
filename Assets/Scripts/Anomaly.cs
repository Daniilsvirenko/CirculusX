using UnityEngine;

public class Anomaly : MonoBehaviour
{
    public enum AnomalyType { Simple, Difficult, Disturbing }

    [Header("GDD Kategorie")]
    public AnomalyType type;

    [Header("Beschreibung")]
    public string description;
}