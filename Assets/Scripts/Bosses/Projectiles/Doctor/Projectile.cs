using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    public float speed = 8f;
    public float lifetime = 10f;
    public int damage = 5;

    private Vector2 direction;
    private float timer;

    public void OnSpawn()
    {
        timer = 0f;

        Player player = Player.Instance;
        if (player != null)
            direction = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
        else
            direction = transform.right;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void OnDespawn()
    {

    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            PoolManager.Instance.Despawn(gameObject);
            return;
        }

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamagable damageable = collision.gameObject.GetComponent<IDamagable>();
            if (damageable != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                damageable.TakeDamage(damage, knockbackDirection, 0f);
            }
        }

        if (collision.CompareTag("Wall"))
        {
            PoolManager.Instance.Despawn(gameObject);
        }
    }
}