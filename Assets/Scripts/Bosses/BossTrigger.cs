using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject Door;
    public GameObject Boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Door.activeSelf) return;

        if (collision.CompareTag("Player"))
        {
            Door.SetActive(true);
            Boss.SetActive(true);
        }
    }

    public void DesactivateDoor() {
        Door.SetActive(false);
    }
}
