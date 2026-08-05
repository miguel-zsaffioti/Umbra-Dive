using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class BossTrigger : MonoBehaviour
{
    public GameObject Door;
    public GameObject boss;
    public Transform SpawnLocation;
    public CinemachineCamera bossArenaCamera;
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
            bossArenaCamera.Priority = 20;
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
        bossArenaCamera.Priority = 1;
        Door.SetActive(false);
        player.RestorePotions();
        gameManager.bossCount++;
        gameManager.activeBossTrigger = null;
        gameManager.SaveGame();
    }
}