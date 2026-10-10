using UnityEngine;

public class OreShard : MonoBehaviour, IPoolable
{
    private Rigidbody2D rb;
    public float speed = 5f;
    public int damage = 3;
    public float lifetime = 5f;

    private bool hasHit;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnSpawn()
    {
        hasHit = false;
        timer = lifetime;
        rb.linearVelocity = (Vector2)transform.right * speed;
    }

    public void OnDespawn()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) Despawn();
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
                damageable.TakeDamage(damage, knockbackDirection, 3f);
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
}