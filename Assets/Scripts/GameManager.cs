using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int _bossCount = 0;
    public int bossCount
    {
        get => _bossCount;
        set
        {
            _bossCount = value;
            OnBossCountChanged?.Invoke();
        }
    }

    public event System.Action OnBossCountChanged;

    [HideInInspector] public BossTrigger activeBossTrigger;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SaveGame()
    {
        Player player = Player.Instance;
        if (player == null) return;

        SaveData data = new SaveData
        {
            bossCount = bossCount,
            playerX = player.transform.position.x,
            playerY = player.transform.position.y,
            currentHealth = player.maxHealth,
            maxHealth = player.maxHealth,
            currentPotions = player.maxPotions,
            maxPotions = player.maxPotions
        };

        SaveManager.Instance.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveManager.Instance.Load();
        if (data == null) return;

        Player player = Player.Instance;
        if (player == null) return;

        bossCount = data.bossCount;

        player.transform.position = new Vector2(data.playerX, data.playerY);
        player.maxHealth = data.maxHealth;
        player.currentHealth = data.currentHealth;
        player.NotifyHealthChanged();

        player.maxPotions = data.maxPotions;
        player.currentPotions = data.currentPotions;
        player.NotifyPotionsChanged();
    }

    // Chamado pelo PlayerHealthSystem quando o player morre
    public void OnPlayerDeath()
    {
        if (activeBossTrigger != null)
        {
            LoadGame();
            Player.Instance.Respawn(Player.Instance.transform.position);
            activeBossTrigger.ResetFight();
        }
        else
        {
            GameOverManager.Instance.ShowGameOver();
        }
    }
}