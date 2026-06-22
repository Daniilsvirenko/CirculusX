using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Handles the final "fade to white" + closing text once the player
// escapes through the exit door in the Delusional Corridor.
public class EndingUIController : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup whiteFadeGroup;     // CanvasGroup on the full-screen white Image
    public CanvasGroup textGroup;          // CanvasGroup on the TMP text object
    public TextMeshProUGUI endText;
    [Tooltip("The player's PlayerMovement component. Gets input-locked for the duration of the ending.")]
    public PlayerMovement playerMovement;

    [Header("Timing")]
    public float fadeToWhiteDuration = 0.5f;
    public float holdWhiteDuration = 0.5f;
    public float lineFadeInDuration = 1.5f;
    public float lineHoldDuration = 10.0f;
    public float lineFadeOutDuration = 1.5f;
    public float pauseBetweenLines = 0.5f;


    [Header("Text")]
    [TextArea(2, 4)]
    public string[] endingLines = new string[]
    {
        "Congratulations on making it this far.\nThe loop is broken, and the air is finally still.\nTake a breath... you are safe now.",
        "Because there was never a hotel.\nThere was only your mind, built into something you could walk through\u2014\na corridor, floor after floor, because facing it all at once was impossible.",
        "Every anomaly you found wasn't a trick of the building.\nIt was a memory you'd buried, a fear you'd avoided,\na version of yourself you didn't want to look at directly.",
        "Getting it wrong didn't reset a level.\nIt meant you weren't ready to see it yet\u2014\nso your mind sent you back to try again.",
        "The ground floor was never a place to escape to.\nIt was the moment you finally stopped looking away.",
        "Well done.\nYou faced the darkness, and you won.\nYou are finally free."
    };

    [Header("Return To Menu")]
    public float promptFadeInDuration = 1.0f;
    [Tooltip("Min/max alpha for the pulsing prompt, once fully faded in.")]
    public float pulseMinAlpha = 0.0f;
    public float pulseMaxAlpha = 1.0f;
    [Tooltip("How long one fade-up or fade-down half of the pulse takes.")]
    public float pulseSpeed = 1.0f;
    [TextArea(1, 2)]
    public string returnPrompt = "<font=\"LiberationSans SDF\"><size=70%><color=#FF0023><lowercase>Press any key to return to the Main Menu</lowercase></color></size></font>";

    private bool hasPlayed = false;
    private bool canReturnToMenu = false;
    private Coroutine pulseCoroutine;

    private void Awake()
    {
        if (whiteFadeGroup != null)
        {
            whiteFadeGroup.alpha = 0f;
            whiteFadeGroup.blocksRaycasts = false;
            whiteFadeGroup.interactable = false;
        }

        if (textGroup != null)
        {
            textGroup.alpha = 0f;
            textGroup.blocksRaycasts = false;
            textGroup.interactable = false;
        }

        if (endText != null) endText.text = "";
    }

    private void Update()
    {
        if (!canReturnToMenu) return;

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            ReturnToMainMenu();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ReturnToMainMenu();
        }
    }

    public void PlayEnding()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        // >>> NEU: Startet die Musik genau jetzt, wenn das Finale beginnt <<<
        if (TryGetComponent<AudioSource>(out AudioSource endingAudio))
        {
            endingAudio.Play();
        }

        if (whiteFadeGroup != null) whiteFadeGroup.blocksRaycasts = true;
        if (textGroup != null) textGroup.blocksRaycasts = true;

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

        // 2. Fade each line in, hold, fade out, then move to the next
        for (int i = 0; i < endingLines.Length; i++)
        {
            if (endText != null) endText.text = endingLines[i];
            if (textGroup != null) textGroup.alpha = 0f;

            // fade in
            t = 0f;
            while (t < lineFadeInDuration)
            {
                t += Time.deltaTime;
                if (textGroup != null)
                    textGroup.alpha = Mathf.Clamp01(t / lineFadeInDuration);
                yield return null;
            }
            if (textGroup != null) textGroup.alpha = 1f;

            // hold
            yield return new WaitForSeconds(lineHoldDuration);

            // fade out
            t = 0f;
            while (t < lineFadeOutDuration)
            {
                t += Time.deltaTime;
                if (textGroup != null)
                    textGroup.alpha = Mathf.Clamp01(1f - (t / lineFadeOutDuration));
                yield return null;
            }
            if (textGroup != null) textGroup.alpha = 0f;

            // pause before next line (skip after the last one)
            if (i < endingLines.Length - 1)
                yield return new WaitForSeconds(pauseBetweenLines);
        }

        // 3. Show the "return to menu" prompt, fade in, then pulse it
        if (endText != null) endText.text = returnPrompt;
        if (textGroup != null) textGroup.alpha = 0f;

        t = 0f;
        while (t < promptFadeInDuration)
        {
            t += Time.deltaTime;
            if (textGroup != null)
                textGroup.alpha = Mathf.Clamp01(t / promptFadeInDuration);
            yield return null;
        }
        if (textGroup != null) textGroup.alpha = pulseMaxAlpha;

        canReturnToMenu = true;
        pulseCoroutine = StartCoroutine(PulsePrompt());
    }

    private IEnumerator PulsePrompt()
    {
        while (canReturnToMenu)
        {
            // fade down
            float t = 0f;
            while (t < pulseSpeed)
            {
                t += Time.deltaTime;
                if (textGroup != null)
                    textGroup.alpha = Mathf.Lerp(pulseMaxAlpha, pulseMinAlpha, t / pulseSpeed);
                yield return null;
            }

            // fade up
            t = 0f;
            while (t < pulseSpeed)
            {
                t += Time.deltaTime;
                if (textGroup != null)
                    textGroup.alpha = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, t / pulseSpeed);
                yield return null;
            }
        }
    }

    private void ReturnToMainMenu()
    {
        canReturnToMenu = false; // prevent double-trigger

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        // >>> NEU: Stoppt Mozart, wenn man zurück ins Menü geht <<<
        if (TryGetComponent<AudioSource>(out AudioSource endingAudio))
        {
            endingAudio.Stop();
        }

        // Fade out this ending canvas's visuals so it doesn't sit on top of the menu
        if (whiteFadeGroup != null) whiteFadeGroup.alpha = 0f;
        if (textGroup != null) textGroup.alpha = 0f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameState();
        }
        else
        {
            Debug.LogWarning("EndingUIController: GameManager.Instance is null, can't reset game state.");
        }

        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ShowStartMenu();
        }
        else
        {
            Debug.LogWarning("EndingUIController: MenuManager.Instance is null, can't return to menu.");
        }

        // Hide the ending canvas itself so it doesn't block input on the menu
        gameObject.SetActive(false);
    }
}