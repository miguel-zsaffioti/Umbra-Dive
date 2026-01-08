using System.Collections;
using UnityEngine;

public class PlayerHealthSystem
{
    Player player;

    public PlayerHealthSystem(Player player)
    {
        this.player = player;
    }

    public void TakeDamage(int damageAmount, Vector2 knockbackDirection, float knockbackForce)
    {
        if (player.isInvincible) return;

        // Checa se está no estado de ataque para aplicar a lógica de Hyper Armor
        bool isAttacking = player.stateMachine.currState == player.attackState;

        // Aplica o dano normalmente
        player.currentHealth -= damageAmount;
        player.audioSystem.PlaySound(player.audioData.damage);

        if (player.currentHealth <= 0)
        {
            Die();
            return;
        }

        // LÓGICA DE HYPER ARMOR:
        // Se estiver atacando, NÃO aplica knockback e NÃO interrompe o ataque
        if (!isAttacking)
        {
            player.ApplyKnockback(knockbackDirection, knockbackForce);
            // A corrotina de invencibilidade visual só deve rodar se não for Hyper Armor
            // ou você pode deixar rodar, mas sem cancelar o ataque.
            player.StartCoroutine(InvincibilityCoroutine());
        }
        else
        {
            // Opcional: Feedback visual que tomou dano mas aguentou (ex: piscar vermelho rápido)
            Debug.Log("Hyper Armor tankou o hit!");
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        yield return null;

        player.isInvincible = true;

        float endTime = Time.time + player.invincibilityDuration;
        while (Time.time < endTime)
        {
            player.spriteRenderer.enabled = !player.spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        player.spriteRenderer.enabled = true;

        player.isInvincible = false;
    }

    private void Die()
    {
        player.gameObject.SetActive(false);
    }

    
}
