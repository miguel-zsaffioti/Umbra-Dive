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
        IDamagable damageable = collision.gameObject.GetComponent<IDamagable>();

        if (damageable != null && enemy != null)
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

            damageable.TakeDamage(enemy.data.damageAmount, knockbackDirection, enemy.data.knockbackForce);
        }
    }

}