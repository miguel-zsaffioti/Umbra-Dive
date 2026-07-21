using UnityEngine;

public class Slash : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 1f;
    public int damage = 3;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {

        float direction = Mathf.Sign(transform.localScale.x);
        rb.linearVelocityX = speed * direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamagable damageable = collision.gameObject.GetComponent<IDamagable>();

            if (damageable != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                damageable.TakeDamage(damage, knockbackDirection, 10f);
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}