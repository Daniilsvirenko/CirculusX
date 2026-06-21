using UnityEngine;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Header("Elevator Settings")]
    [Tooltip("Check this if this is the START elevator (used when an anomaly is suspected)")]
    public bool isStartElevator;

    [Header("Interaction Prompt")]
    public string promptText = "Press E";
    public string PromptText => promptText;

    public void Interact()
    {
        GameManager.Instance.MakeDecision(isStartElevator);
    }
}