using Unity.VisualScripting;
using UnityEngine;

public class FrontAttack : MonoBehaviour
{
    private bool activated = false;
    public int damage = 3;

    private bool hasDamagedThisActivation = false;

    public void Activate()
    {
        activated = true;
    }
    
    public void Deactivate() { 
        activated = false;
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!activated || hasDamagedThisActivation) return;
        if (collision.CompareTag("Player"))
        {
            IDamagable damageable = collision.gameObject.GetComponent<IDamagable>();
            if (damageable != null)
            {
                hasDamagedThisActivation = true;
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                damageable.TakeDamage(damage, knockbackDirection, 10f);
            }
        }
    }
}
