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
        if (collision.CompareTag("Player")) {
            Debug.Log("Acertou o jogador");
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall")) { 
            Destroy(gameObject);
        }
    }
}
