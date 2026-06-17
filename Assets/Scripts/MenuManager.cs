using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public bool IsPaused { get; private set; } = false;
    public bool GameStarted { get; private set; } = false;

    [Header("UI Panels")]
    [Tooltip("The main full-screen panel with the background/blur.")]
    public GameObject menuPanel; 
    public GameObject startButton;
    public GameObject resumeButton;
    public GameObject quitButton;
    public GameObject titleText;

    [Header("Post Processing")]
    public Volume blurVolume;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // On start, show the start menu and pause the game
        ShowStartMenu();
    }

    private void Update()
    {
        // Передаем реальное время в шейдер, чтобы вода/гипноз работали на паузе
        Shader.SetGlobalFloat("_GlobalUnscaledTime", Time.unscaledTime);

        if (IsPaused)
        {
            UnlockCursor(); // Гарантируем, что курсор свободен
        }

        // Only allow toggling pause if the game has already started
        if (GameStarted && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ShowStartMenu()
    {
        IsPaused = true;
        GameStarted = false;
        Time.timeScale = 0.0001f; // Используем микро-значение вместо 0, чтобы обойти баг Input System
        AudioListener.pause = true; // Пауза для всех звуков

        if (menuPanel != null) menuPanel.SetActive(true);
        if (startButton != null) startButton.SetActive(true);
        if (resumeButton != null) resumeButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(true);
        if (titleText != null) titleText.SetActive(true);
        if (blurVolume != null) blurVolume.weight = 1f;

        UnlockCursor();
    }

    // Call this from the OnClick event of the Start Button
    public void OnStartButtonClicked()
    {
        GameStarted = true;
        ResumeGame();
    }

    // Call this from the OnClick event of the Resume Button
    public void OnResumeButtonClicked()
    {
        ResumeGame();
    }

    // Call this from the OnClick event of the Quit Button
    public void OnQuitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0.0001f; // Используем микро-значение вместо 0
        AudioListener.pause = true; // Пауза для всех звуков

        if (menuPanel != null) menuPanel.SetActive(true);
        if (startButton != null) startButton.SetActive(false);
        if (resumeButton != null) resumeButton.SetActive(true);
        if (quitButton != null) quitButton.SetActive(true);
        if (titleText != null) titleText.SetActive(false);
        if (blurVolume != null) blurVolume.weight = 1f;

        UnlockCursor();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false; // Включаем звуки обратно

        if (menuPanel != null) menuPanel.SetActive(false);
        if (blurVolume != null) blurVolume.weight = 0f;

        LockCursor();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
