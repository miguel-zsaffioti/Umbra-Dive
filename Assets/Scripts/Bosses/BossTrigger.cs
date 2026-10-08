using UnityEngine;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;
using TMPro;

public class BossTrigger : MonoBehaviour
{
    public GameObject Door;
    public GameObject boss;
    public Transform SpawnLocation;
    public CinemachineCamera bossArenaCamera;
    public int bossCameraActivePriority = 20;
    private PrioritySettings bossCameraIdlePriority;

    [Header("Pixel Perfect (zoom da luta)")]
    public PixelPerfectCamera pixelPerfectCamera;
    public Vector2Int bossRefResolution = new Vector2Int(2560, 1440);
    private Vector2Int originalRefResolution;

    private Transform Parent;
    public GameObject healthPanel;
    public TextMeshProUGUI bossName;
    public Collider2D arenaBounds;
    public GameManager gameManager;
    public int bossOrderer;
    private Player player;
    private bool activated;
    private GameObject spawnedBoss;

    private void Start()
    {
        Parent = transform.parent;
        activated = false;

        if (bossArenaCamera != null)
        {
            bossCameraIdlePriority = bossArenaCamera.Priority;
        }

        if (pixelPerfectCamera == null && Camera.main != null)
        {
            pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();
        }

        if (pixelPerfectCamera != null)
        {
            originalRefResolution = new Vector2Int(
                pixelPerfectCamera.refResolutionX,
                pixelPerfectCamera.refResolutionY
            );
        }

        Refresh();
    }

    private void OnEnable()
    {
        if (gameManager != null)
        {
            gameManager.OnBossCountChanged += Refresh;
        }
    }

    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.OnBossCountChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        bool isCurrentBoss = gameManager.bossCount == bossOrderer;
        bool alreadyDefeated = gameManager.bossCount > bossOrderer;

        if (alreadyDefeated)
        {
            activated = true;
            Door.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
        }
        else if (isCurrentBoss)
        {
            activated = false;
            Door.SetActive(false);
            GetComponent<Collider2D>().enabled = true;
        }
        else
        {
            Door.SetActive(true);
            GetComponent<Collider2D>().enabled = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;
        if (gameManager.bossCount != bossOrderer) return;
        if (SpawnLocation == null) return;
        if (Parent == null) return;
        if (arenaBounds == null) return;

        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Player>();
            Door.SetActive(true);

            gameManager.activeBossTrigger = this;
            gameManager.SaveGame();

            SpawnBoss();

            activated = true;
            healthPanel.SetActive(true);
        }
    }

    private void SpawnBoss()
    {
        spawnedBoss = Instantiate(boss, SpawnLocation.position, boss.transform.rotation, Parent);
        Boss bossScript = spawnedBoss.GetComponent<Boss>();
        if (bossScript != null)
        {
            bossScript.trigger = this;
            bossName.text = bossScript.bossName;
            bossScript.arenaBounds = arenaBounds;
        }
        if (bossArenaCamera != null)
        {
            bossArenaCamera.Priority = bossCameraActivePriority;
        }
        if (pixelPerfectCamera != null)
        {
            pixelPerfectCamera.refResolutionX = bossRefResolution.x;
            pixelPerfectCamera.refResolutionY = bossRefResolution.y;
        }
    }

    public void ResetFight()
    {
        if (spawnedBoss != null)
        {
            Destroy(spawnedBoss);
        }

        healthPanel.SetActive(true);
        SpawnBoss();
    }

    public void EndBossFight()
    {
        healthPanel.SetActive(false);
        if (bossArenaCamera != null)
        {
            bossArenaCamera.Priority = bossCameraIdlePriority;
        }
        if (pixelPerfectCamera != null)
        {
            pixelPerfectCamera.refResolutionX = originalRefResolution.x;
            pixelPerfectCamera.refResolutionY = originalRefResolution.y;
        }
        Door.SetActive(false);
        player.RestorePotions();
        gameManager.bossCount++;
        gameManager.activeBossTrigger = null;
        gameManager.SaveGame();
    }
}