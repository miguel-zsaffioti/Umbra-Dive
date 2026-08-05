using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public string menuSceneName = "MainMenu";
    private bool isPaused = false;
    public GameObject pausePanel;

    public static event System.Action<bool> OnPauseStateChanged; // true = pausado

    void Update()
    {
        if (GameOverManager.IsGameOver) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else Pause();
        }
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