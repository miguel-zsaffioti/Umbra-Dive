using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public string menuSceneName = "MainMenu";
    private bool isPaused = false;
    public GameObject pausePanel;

    public static event System.Action<bool> OnPauseStateChanged;

    void Update()
    {
        if (GameOverManager.IsGameOver) return;

        if (PausePressed())
        {
            if (isPaused) ResumeGame();
            else Pause();
        }
    }

    private bool PausePressed()
    {
        bool keyboard = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool gamepad = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;
        return keyboard || gamepad;
    }

    public void Pause()
    {
        if (GameOverManager.IsGameOver) return;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        OnPauseStateChanged?.Invoke(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        isPaused = false;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        OnPauseStateChanged?.Invoke(false);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}