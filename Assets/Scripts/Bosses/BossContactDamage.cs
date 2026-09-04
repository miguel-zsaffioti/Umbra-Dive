using UnityEngine;

public class BossContactDamage : MonoBehaviour
{
    public int damageAmount = 1;
    public float knockbackForce = 2f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        IDamagable damageable = collision.gameObject.GetComponent<IDamagable>();

        if (damageable != null)
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

            damageable.TakeDamage(damageAmount, knockbackDirection, knockbackForce);
        }
    }
}