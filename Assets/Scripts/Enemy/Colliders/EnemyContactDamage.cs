using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{

    Enemy enemy;

    private void Start()
    {
        enemy = gameObject.GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null && enemy != null)
        {
            PlayerHealthSystem playerHealth = player.healthSystem;

            if (playerHealth != null)
            {
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                 
                playerHealth.TakeDamage(enemy.data.damageAmount, knockbackDirection, enemy.data.knockbackForce);
            }
        }
    }

}
