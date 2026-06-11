using UnityEngine;

public class ElevatorCallButton : MonoBehaviour, IInteractable
{
    [Tooltip("Drag the parent ElevatorAnimation object here. If left empty, it will try to find it automatically.")]
    public ElevatorDoorController doorController;

    void Start()
    {
        if (doorController == null)
        {
            // Try to find it in the parent hierarchy
            doorController = GetComponentInParent<ElevatorDoorController>();
        }
    }

    public void Interact()
    {
        if (doorController != null)
        {
            Debug.Log("Elevator Called!");
            doorController.OpenDoors();
        }
        else
        {
            Debug.LogWarning("ElevatorDoorController is not assigned or found on parent!");
        }
    }
}
