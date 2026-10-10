using UnityEngine;

public class Slash : MonoBehaviour, IPoolable
{
    private Rigidbody2D rb;
    public float speed = 1f;
    public int damage = 3;

    private bool hasHit; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(float direction)
    {
        direction = Mathf.Sign(direction);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        rb.linearVelocityX = speed * direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("Player"))
        {
            IDamagable damageable = collision.gameObject.GetComponent<IDamagable>();

            if (damageable != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                damageable.TakeDamage(damage, knockbackDirection, 10f);
            }

            Despawn();
        }
        else if (collision.CompareTag("Wall"))
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        hasHit = true;
        PoolManager.Instance.Despawn(gameObject);
    }

    public void OnSpawn()
    {
        hasHit = false;
    }

    public void OnDespawn()
    {
        rb.linearVelocity = Vector2.zero;
    }
}