using UnityEngine;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Header("Elevator Settings")]
    [Tooltip("Check this if this is the START elevator (used when an anomaly is suspected)")]
    public bool isStartElevator;

    public void Interact()
    {
        // Обновляем точку спавна так, чтобы она стала там, где игрок стоит прямо сейчас.
        // Теперь при переходе на следующий этаж (или при рестарте) игрок останется в этом же лифте!
        if (GameManager.Instance != null && GameManager.Instance.player != null && GameManager.Instance.spawnPoint != null)
        {
            GameManager.Instance.spawnPoint.position = GameManager.Instance.player.position;
            GameManager.Instance.spawnPoint.rotation = GameManager.Instance.player.rotation;
        }

        // Tell the GameManager what the player chose
        GameManager.Instance.MakeDecision(isStartElevator);
    }
}