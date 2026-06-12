using UnityEngine;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Header("Elevator Settings")]
    [Tooltip("Check this if this is the START elevator (used when an anomaly is suspected)")]
    public bool isStartElevator;

    public void Interact()
    {
        // Tell the GameManager what the player chose
        GameManager.Instance.MakeDecision(isStartElevator);
    }
}