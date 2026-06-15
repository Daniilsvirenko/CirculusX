using UnityEngine;

public class ElevatorCallButton : MonoBehaviour, IInteractable
{
    [Tooltip("Drag the parent ElevatorAnimation object here. If left empty, it will try to find it automatically.")]
    public ElevatorDoorController doorController;

    [Header("Audio Settings")]
    public AudioSource buttonAudioSource;
    public AudioClip clickSound;
    [Tooltip("С какой секунды звукового файла начать воспроизведение (обрезаем начало)")]
    public float audioStartSecond = 0f;
    [Tooltip("Через сколько секунд выключить звук, если он слишком длинный (0 = играть до конца)")]
    public float audioDuration = 0f;

    void Start()
    {
        if (doorController == null)
        {
            // Try to find it in the parent hierarchy
            doorController = GetComponentInParent<ElevatorDoorController>();
        }

        if (buttonAudioSource == null)
        {
            // Try to get AudioSource from this object
            buttonAudioSource = GetComponent<AudioSource>();
        }
    }

    public void Interact()
    {
        // Play click sound with trim
        if (buttonAudioSource != null && clickSound != null)
        {
            StopAllCoroutines();
            StartCoroutine(PlayButtonSoundRoutine());
        }

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

    private System.Collections.IEnumerator PlayButtonSoundRoutine()
    {
        buttonAudioSource.clip = clickSound;
        buttonAudioSource.time = audioStartSecond;
        buttonAudioSource.Play();

        if (audioDuration > 0)
        {
            yield return new WaitForSeconds(audioDuration);
            buttonAudioSource.Stop();
        }
    }
}
