using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Game";

    [Header("Botões")]
    public GameObject continueButton;

    private void Start()
    {
        AudioManager.Instance.Play(AudioManager.Instance.menu);
        if (continueButton != null)
        {
            continueButton.SetActive(SaveManager.SaveExists());
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;

        if (SaveManager.SaveExists())
        {
            SaveManager.Instance.DeleteSave();
        }
        AudioManager.Instance.Play(AudioManager.Instance.ambient);
        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;

        if (!SaveManager.SaveExists())
        {
            PlayGame();
            return;
        }
        AudioManager.Instance.Play(AudioManager.Instance.ambient);
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