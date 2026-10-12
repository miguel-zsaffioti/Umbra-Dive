using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class ControlsScreen : MonoBehaviour
{
    public static bool ContinueRequested;

    public string gameSceneName = "MainScene";
    public float inputDelay = 0.5f;

    private float timer;
    private bool starting;

    private void Start()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (starting) return;

        timer += Time.unscaledDeltaTime;
        if (timer < inputDelay) return;

        if (AnyInputPressed())
        {
            starting = true;
            StartGame();
        }
    }

    private static bool AnyInputPressed()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) return true;


        if (Mouse.current != null &&
            (Mouse.current.leftButton.wasPressedThisFrame ||
             Mouse.current.rightButton.wasPressedThisFrame ||
             Mouse.current.middleButton.wasPressedThisFrame))
        {
            return true;
        }


        foreach (var pad in Gamepad.all)
        {
            foreach (var control in pad.allControls)
            {
                if (control is ButtonControl button && button.wasPressedThisFrame) return true;
            }
        }

        return false;
    }

    private void StartGame()
    {
        bool hasSave = SaveManager.SaveExists();

        if (ContinueRequested && hasSave)
        {
            SceneManager.sceneLoaded += OnGameSceneLoaded;
        }
        else if (hasSave && SaveManager.Instance != null)
        {
            // Jogo novo: apaga o save antigo
            SaveManager.Instance.DeleteSave();
        }

        ContinueRequested = false;
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
}