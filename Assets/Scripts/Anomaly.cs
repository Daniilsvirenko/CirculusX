using UnityEngine;

public class Anomaly : MonoBehaviour
{
    public enum AnomalyType { Simple, Difficult, Disturbing }
    // Defines how this anomaly behaves when active
    public enum AnomalyBehavior { HideWhenAnomalyPresent, ShowWhenAnomalyPresent }

    [Header("GDD Category")]
    public AnomalyType type;
    public AnomalyBehavior behavior;

    [Header("Description")]
    public string description;

    // (Убрана автоматическая регистрация Awake). 
    // Теперь аномалии регистрируются ТОЛЬКО если они добавлены в AnomalyManager!
}