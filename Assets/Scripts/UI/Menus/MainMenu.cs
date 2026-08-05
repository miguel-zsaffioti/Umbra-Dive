using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Game";
    public TextMeshProUGUI playButtonText;
    public float playFontSize = 40f;
    public float continueFontSize = 30f;
    private void Start()
    {
        if (playButtonText != null)
        {
            bool hasSave = SaveManager.SaveExists();
            playButtonText.text = hasSave ? "Continue" : "Play";
            playButtonText.fontSize = hasSave ? continueFontSize : playFontSize;
        }
    }
    public void PlayGame()
    {
        Time.timeScale = 1f;
        if (SaveManager.SaveExists())
        {
            SceneManager.sceneLoaded += OnGameSceneLoaded;
        }
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