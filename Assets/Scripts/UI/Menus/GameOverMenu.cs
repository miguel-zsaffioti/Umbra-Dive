using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance; // (mantém como já era: public static GameOverManager Instance;)
    public string menuSceneName = "MainMenu";
    public GameObject gameOverPanel;
    public CanvasGroup gameOverCanvasGroup;
    public float fadeDuration = 0.4f;

    public static event System.Action OnGameOverTriggered;

    void Awake()
    {
        Instance = this;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(FadeIn());
        OnGameOverTriggered?.Invoke();
    }

    private IEnumerator FadeIn()
    {
        gameOverCanvasGroup.alpha = 0f;
        gameOverCanvasGroup.interactable = false;
        gameOverCanvasGroup.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            gameOverCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        gameOverCanvasGroup.alpha = 1f;
        gameOverCanvasGroup.interactable = true;
    }

    public void LoadCheckpoint()
    {
        Time.timeScale = 1f;
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