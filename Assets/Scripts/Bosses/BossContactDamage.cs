using UnityEngine;

public class BossContactDamage : MonoBehaviour
{
    public int damageAmount = 1;
    public float knockbackForce = 2f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            PlayerHealthSystem playerHealth = player.healthSystem;

            if (playerHealth != null)
            {
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;

                playerHealth.TakeDamage(damageAmount, knockbackDirection, knockbackForce);
            }
        }
    }
}
