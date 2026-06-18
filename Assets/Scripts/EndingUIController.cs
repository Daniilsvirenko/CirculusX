using UnityEngine;
using TMPro;
using System.Collections;

// Handles the final "fade to white" + closing text once the player
// escapes through the exit door in the Delusional Corridor.
// Setup: put this on a Canvas (Screen Space - Overlay) with:
//  - a full-screen white Image, alpha 0, referenced as "whiteImage"
//  - a TMP text ("THE END" / explanation), alpha 0, referenced as "endText"
public class EndingUIController : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup whiteFadeGroup;     // CanvasGroup on the full-screen white Image
    public CanvasGroup textGroup;          // CanvasGroup on the TMP text object
    public TextMeshProUGUI endText;
    [Tooltip("The player's PlayerMovement component. Gets input-locked for the duration of the ending.")]
    public PlayerMovement playerMovement;

    [Header("Timing")]
    public float fadeToWhiteDuration = 2.5f;
    public float holdWhiteDuration = 1.0f;
    public float textFadeInDuration = 2.0f;

    [Header("Text")]
    [TextArea(3, 6)]
    public string endingMessage =
        "THE END\n\nThe corridor was never a place.\nIt was a thought, repeated until it broke.\nYou stopped running from it.";

    private bool hasPlayed = false;

    private void Awake()
    {
        if (whiteFadeGroup != null) whiteFadeGroup.alpha = 0f;
        if (textGroup != null) textGroup.alpha = 0f;
        if (endText != null) endText.text = "";
    }

    public void PlayEnding()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        if (playerMovement != null)
        {
            playerMovement.SetInputLocked(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        // 1. Fade to white
        float t = 0f;
        while (t < fadeToWhiteDuration)
        {
            t += Time.deltaTime;
            if (whiteFadeGroup != null)
                whiteFadeGroup.alpha = Mathf.Clamp01(t / fadeToWhiteDuration);
            yield return null;
        }
        if (whiteFadeGroup != null) whiteFadeGroup.alpha = 1f;

        yield return new WaitForSeconds(holdWhiteDuration);

        // 2. Fade in the closing text
        if (endText != null) endText.text = endingMessage;

        t = 0f;
        while (t < textFadeInDuration)
        {
            t += Time.deltaTime;
            if (textGroup != null)
                textGroup.alpha = Mathf.Clamp01(t / textFadeInDuration);
            yield return null;
        }
        if (textGroup != null) textGroup.alpha = 1f;
    }
}
