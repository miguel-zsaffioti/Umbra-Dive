using UnityEngine;

public class EnemyHealthSystem
{
    Enemy enemy;
    private FlashEffect flashEffect;

    public EnemyHealthSystem(Enemy enemy)
    {
        this.enemy = enemy;
        this.flashEffect = enemy.GetComponent<FlashEffect>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (enemy.isDead) return;

        enemy.currentHealth -= damage;

        // ... flash effect ...

        if (enemy.currentHealth <= 0)
        {
            StartDying();
        }
        else
        {
            enemy.ApplyKnockback(knockbackDirection, knockbackForce);
            enemy.animator.SetTrigger("Hurt");
        }
    }

    private void StartDying()
    {
        enemy.isDead = true;
        enemy.animator.SetTrigger("Die");

        enemy.rb.linearVelocity = Vector2.zero;
        enemy.GetComponent<Collider2D>().enabled = false;

        enemy.enabled = false;
    }
}