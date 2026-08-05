using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI potionCountText;
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
        subscribedPlayer.OnPotionsChanged += UpdatePotionUI;

        UpdatePotionUI(subscribedPlayer.currentPotions, subscribedPlayer.maxPotions);
    }

    private void OnDisable()
    {
        if (subscribedPlayer != null)
        {
            subscribedPlayer.OnPotionsChanged -= UpdatePotionUI;
            subscribedPlayer = null;
        }

        PauseMenu.OnPauseStateChanged -= HandlePauseChanged;
        GameOverManager.OnGameOverTriggered -= HandleGameOver;
    }

    private void UpdatePotionUI(int current, int max)
    {
        potionCountText.text = $"{current}/{max}";
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