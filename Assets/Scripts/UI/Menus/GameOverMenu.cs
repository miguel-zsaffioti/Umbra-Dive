using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;
    public static bool IsGameOver { get; private set; }

    public string menuSceneName = "MainMenu";
    public string gameSceneName = "MainScene";
    public GameObject gameOverPanel;
    public CanvasGroup gameOverCanvasGroup;

    [Header("Fade")]
    public float fadeDuration = 0.4f;
    public bool syncFadeWithSound = true;
    public float maxSyncedFadeDuration = 3f;

    public static event System.Action OnGameOverTriggered;

    void Awake()
    {
        Instance = this;
    }

    public void ShowGameOver()
    {
        IsGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        float duration = fadeDuration;

        if (AudioManager.Instance != null)
        {
            float soundLength = AudioManager.Instance.PlayOnce(AudioManager.Instance.gameOver);

            if (syncFadeWithSound && soundLength > 0f)
            {
                duration = Mathf.Min(soundLength, maxSyncedFadeDuration);
            }
        }

        StartCoroutine(FadeIn(duration));
        OnGameOverTriggered?.Invoke();
    }

    private IEnumerator FadeIn(float duration)
    {
        gameOverCanvasGroup.alpha = 0f;
        gameOverCanvasGroup.interactable = false;
        gameOverCanvasGroup.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            gameOverCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        gameOverCanvasGroup.alpha = 1f;
        gameOverCanvasGroup.interactable = true;
    }

    public void LoadCheckpoint()
    {
        IsGameOver = false;
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(AudioManager.Instance.ambient);
        }


        if (!SaveManager.SaveExists())
        {
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnGameSceneLoaded;
        GameManager.Instance.StartCoroutine(LoadAfterSceneReady());
    }

    private static IEnumerator LoadAfterSceneReady()
    {
        yield return null;

        GameManager.Instance.LoadGame();
        Player.Instance.Respawn(Player.Instance.transform.position);

        if (GameManager.Instance.activeBossTrigger != null)
        {
            GameManager.Instance.activeBossTrigger.ResetFight();
        }
    }

    public void BackToMenu()
    {
        IsGameOver = false;
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