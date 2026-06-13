using UnityEngine;

public class GramophoneAnomalyController : MonoBehaviour
{
    [Header("Audio Target")]
    public AudioSource gramophoneSource;

    [Header("Audio Tracks")]
    public AudioClip normalTrack;
    public AudioClip anomalyTrack;

    // This runs the exact frame the GameManager activates this anomaly
    private void OnEnable()
    {
        if (gramophoneSource != null && anomalyTrack != null)
        {
            SwapTrack(anomalyTrack);
        }
    }

    // This runs the exact frame the GameManager resets the loop to normal
    private void OnDisable()
    {
        if (gramophoneSource != null && normalTrack != null)
        {
            SwapTrack(normalTrack);
        }
    }

    private void SwapTrack(AudioClip newClip)
    {
        // Don't restart the audio source if it's already playing the right track
        if (gramophoneSource.clip == newClip) return;

        gramophoneSource.Stop();
        gramophoneSource.clip = newClip;
        gramophoneSource.Play();
    }
}