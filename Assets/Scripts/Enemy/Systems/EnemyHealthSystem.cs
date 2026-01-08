using UnityEditor;
using UnityEngine;

public class EnemyHealthSystem
{
    Enemy enemy;

    // Otimização: Cache do componente para não usar GetComponent toda vez (opcional, mas recomendado)
    private FlashEffect flashEffect;

    public EnemyHealthSystem(Enemy enemy)
    {
        this.enemy = enemy;
        // Tenta pegar o componente FlashEffect que está no Inimigo
        this.flashEffect = enemy.GetComponent<FlashEffect>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (enemy.isDead) return; // Se já morreu, ignora dano novo

        enemy.currentHealth -= damage;

        // ... flash effect ...

        if (enemy.currentHealth <= 0)
        {
            StartDying();
        }
        else
        {
            // Só aplica knockback e hurt se NÃO morreu
            enemy.ApplyKnockback(knockbackDirection, knockbackForce);
            enemy.animator.SetTrigger("Hurt"); // Importante ter animação de hit
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