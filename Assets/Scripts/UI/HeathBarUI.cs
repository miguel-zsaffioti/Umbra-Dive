using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

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
    }

    private void UpdateHealthBar(int current, int max)
    {
        Debug.Log($"[HealthBarUI] Atualizando barra: {current}/{max}, fillImage={(fillImage != null ? fillImage.name : "NULL")}");
        fillImage.fillAmount = (float)current / max;
    }
}