using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI potionCountText;

    private Player subscribedPlayer;

    private void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
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
    }

    private void UpdatePotionUI(int current, int max)
    {
        potionCountText.text = $"{current}/{max}";
    }
}