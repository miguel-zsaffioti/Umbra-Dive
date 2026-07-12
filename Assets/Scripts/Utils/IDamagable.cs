using UnityEngine;

public interface IDamagable
{
    void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce);
}
