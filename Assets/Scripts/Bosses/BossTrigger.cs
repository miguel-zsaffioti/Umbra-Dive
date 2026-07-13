using UnityEngine;
using Unity.Cinemachine;

public class BossTrigger : MonoBehaviour
{
    public GameObject Door;
    public GameObject boss;
    public Transform SpawnLocation;
    public CinemachineCamera bossArenaCamera; // NOVO - arraste a câmera fixa da arena aqui
    private Transform Parent;

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
            Door.SetActive(true);
            GameObject spawnedBoss = Instantiate(boss, SpawnLocation.position, boss.transform.rotation, Parent);

            Boss bossScript = spawnedBoss.GetComponent<Boss>();
            if (bossScript != null)
            {
                bossScript.trigger = this;
            }

            if (bossArenaCamera != null)
            {
                bossArenaCamera.Priority = 20;
            }

            activated = true;
        }
    }

    public void DesactivateDoor()
    {
        Door.SetActive(false);

        if (bossArenaCamera != null)
        {
            bossArenaCamera.Priority = 0;
        }
    }
}