using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject barRoot;
    private Player subscribedPlayer;

    private void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
        PauseMenu.OnPauseStateChanged += HandlePauseChanged;
        GameOverManager.OnGameOverTriggered += HandleGameOver;
    }

    private IEnumerator SubscribeWhenReady()
    {
        while (Player.Instance == null)
        {
            yield return null;
        }

        subscribedPlayer = Player.Instance;
        subscribedPlayer.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(subscribedPlayer.currentHealth, subscribedPlayer.maxHealth);
    }

    private void OnDisable()
    {
        if (subscribedPlayer != null)
        {
            subscribedPlayer.OnHealthChanged -= UpdateHealthBar;
            subscribedPlayer = null;
        }

        PauseMenu.OnPauseStateChanged -= HandlePauseChanged;
        GameOverManager.OnGameOverTriggered -= HandleGameOver;
    }

    private void UpdateHealthBar(int current, int max)
    {
        fillImage.fillAmount = (float)current / max;
    }

    private void HandlePauseChanged(bool isPaused)
    {
        barRoot.SetActive(!isPaused);
    }

    private void HandleGameOver()
    {
        barRoot.SetActive(false);
    }
}