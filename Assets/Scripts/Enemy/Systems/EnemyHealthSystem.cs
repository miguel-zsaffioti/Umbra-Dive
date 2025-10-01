using UnityEditor;
using UnityEngine;

public class EnemyHealthSystem
{
    Enemy enemy;

    public EnemyHealthSystem(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        enemy.currentHealth -= damage;
        enemy.animator.SetTrigger("Hurt");

        enemy.ApplyKnockback(knockbackDirection, knockbackForce);
        if (enemy.currentHealth <= 0)
        {
            StartDying();
            return;
        }
    }

    private void StartDying()
    {

        enemy.animator.SetTrigger("Die");
    }

}
