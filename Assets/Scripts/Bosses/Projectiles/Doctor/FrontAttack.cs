using UnityEngine;

public class FrontAttack : MonoBehaviour, IPoolable
{
    private bool activated = false;
    public int damage = 3;

    private bool hasDamagedThisActivation = false;

    public void OnSpawn()
    {
        activated = false;
        hasDamagedThisActivation = false;
    }

    public void OnDespawn()
    {
        activated = false;
    }

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
        PoolManager.Instance.Despawn(gameObject);
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