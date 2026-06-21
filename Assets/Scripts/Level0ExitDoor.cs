using UnityEngine;

// Attach this to the Exit door inside the Delusional Corridor (Level 0).
// Works the same way as ElevatorButton: implements IInteractable so the
// existing PlayerInteractor / "E" key handling picks it up automatically.
public class Level0ExitDoor : MonoBehaviour, IInteractable
{
    [Header("Interaction Prompt")]
    public string promptText = "Press E to open the door";
    public string PromptText => promptText;

    public void Interact()
    {
        GameManager.Instance.TriggerWinEnding();
    }
}