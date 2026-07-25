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

    private Player player;

    private bool activated;

    private void Start()
    {
        Parent = transform.parent;
        activated = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;
        if (Door.activeSelf) return;
        if (SpawnLocation == null) return;
        if (Parent == null) return;

        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Player>();
            Door.SetActive(true);
            GameObject spawnedBoss = Instantiate(boss, SpawnLocation.position, boss.transform.rotation, Parent);

            Boss bossScript = spawnedBoss.GetComponent<Boss>();
            if (bossScript != null)
            {
                bossScript.trigger = this;
                bossName.text = bossScript.bossName;
            }

            if (bossArenaCamera != null)
            {
                bossArenaCamera.Priority = 20;
            }

            activated = true;
            healthPanel.SetActive(true);
        }
    }

    public void EndBossFight()
    {
        healthPanel.SetActive(false);
        bossArenaCamera.Priority = 1;
        Door.SetActive(false);
        player.RestoreHealth();
    }
}