using System.Collections;
using UnityEngine;

public class EnemyHealthSystem
{
    Enemy enemy;
    private FlashEffect flashEffect;
    private DeathEffect deathEffect;

    public EnemyHealthSystem(Enemy enemy)
    {
        this.enemy = enemy;
        this.flashEffect = enemy.GetComponent<FlashEffect>();
        this.deathEffect = enemy.GetComponent<DeathEffect>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (enemy.isDead) return;

        enemy.currentHealth -= damage;

        if (enemy.currentHealth <= 0)
        {
            StartDying();
        }
        else
        {
            if (flashEffect != null)
            {
                flashEffect.Flash();
            }

            enemy.ApplyKnockback(knockbackDirection, knockbackForce);
            //enemy.animator.SetTrigger("Hurt");
        }
    }

    private void StartDying()
    {
        enemy.isDead = true;
        //enemy.animator.SetTrigger("Die");

        enemy.rb.linearVelocity = Vector2.zero;
        enemy.GetComponent<Collider2D>().enabled = false;

        if (deathEffect != null)
        {
            deathEffect.StartCoroutine(WaitForDeathEffect());
        }
        else
        {
            enemy.Die();
        }

        enemy.enabled = false;
    }

    private IEnumerator WaitForDeathEffect()
    {
        yield return deathEffect.PlayDeathEffectInstant();
        enemy.Die();
    }
}