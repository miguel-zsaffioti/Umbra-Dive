using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject barRoot; 
    private Boss subscribedBoss;

    private void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
        PauseMenu.OnPauseStateChanged += HandlePauseChanged;
        GameOverManager.OnGameOverTriggered += HandleGameOver;
    }

    private IEnumerator SubscribeWhenReady()
    {
        while (Boss.Instance == null)
        {
            yield return null;
        }

        subscribedBoss = Boss.Instance;
        subscribedBoss.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(subscribedBoss.CurrentHealth, subscribedBoss.maxHealth);
    }

    private void OnDisable()
    {
        if (subscribedBoss != null)
        {
            subscribedBoss.OnHealthChanged -= UpdateHealthBar;
            subscribedBoss = null;
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