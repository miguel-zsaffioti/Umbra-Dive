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
        if (player.isInvincible) { return; }

        player.currentHealth -= damageAmount;

        player.ApplyKnockback(knockbackDirection, knockbackForce);

        player.StartCoroutine(InvincibilityCoroutine());

        if (player.currentHealth <= 0)
        {
            Die();
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
