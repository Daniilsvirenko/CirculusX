using UnityEngine;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Header("Elevator Settings")]
    [Tooltip("Check this if this is the START elevator (used when an anomaly is suspected)")]
    public bool isStartElevator;

    public void Interact()
    {
        // Tell the GameManager what the player chose
        // If they chose the Start Elevator, it means they suspect an anomaly (true)
        // If they chose the End Elevator, they think it's normal (false)
        GameManager.Instance.MakeDecision(isStartElevator);
    }
}