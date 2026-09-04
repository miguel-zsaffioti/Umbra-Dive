using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;         
    public float speed = 8f;
    public float turnSpeed = 120f;    
    public float homingDuration = 2f;

    public int damage = 5;

    private Vector2 currentDirection;
    private float timer;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        if (target != null)
        {
            currentDirection = (target.position - transform.position).normalized;
        }
        else
        {
            currentDirection = transform.right;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (target != null && timer < homingDuration)
        {
            Vector2 desiredDirection = (target.position - transform.position).normalized;

            float maxRadiansDelta = turnSpeed * Mathf.Deg2Rad * Time.deltaTime;
            currentDirection = Vector3.RotateTowards(currentDirection, desiredDirection, maxRadiansDelta, 0f);
        }

        transform.position += (Vector3)(currentDirection * speed * Time.deltaTime);

        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
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
            Destroy(gameObject);
        }
    }
}