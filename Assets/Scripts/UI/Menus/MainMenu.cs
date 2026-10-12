using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string controlsSceneName = "Controls";

    [Header("Botões")]
    public GameObject continueButton;

    private void Start()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(SaveManager.SaveExists());
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        ControlsScreen.ContinueRequested = false;
        SceneManager.LoadScene(controlsSceneName);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;


        ControlsScreen.ContinueRequested = SaveManager.SaveExists();
        SceneManager.LoadScene(controlsSceneName);
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