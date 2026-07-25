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
        if (player.isBlocking)
        {
            Vector2 snappedDirection = new Vector2(
            Mathf.Round(knockbackDirection.x),
            Mathf.Round(knockbackDirection.y));

            if (-snappedDirection.x == player.animator.GetFloat("LastX") &&
                -snappedDirection.y == player.animator.GetFloat("LastY"))
            {
                //Debug.Log("Bloqueou");
                player.ApplyKnockback(knockbackDirection, knockbackForce * 2f);
                player.StartCoroutine(InvincibilityCoroutine(false));
                return;
            }
        }

        bool isAttacking = player.stateMachine.currState == player.attackState;

        player.currentHealth -= damageAmount;
        player.audioSystem.PlaySound(player.audioData.damage);
        player.NotifyHealthChanged();

        if (player.currentHealth <= 0)
        {
            Die();
            return;
        }

        if (!isAttacking)
        {
            player.ApplyKnockback(knockbackDirection, knockbackForce);
            player.StartCoroutine(InvincibilityCoroutine(true));
        }
    }

    private IEnumerator InvincibilityCoroutine(bool flashSprite)
    {
        player.isInvincible = true;

        float endTime = Time.time + player.invincibilityDuration;

        while (Time.time < endTime)
        {
            if (flashSprite)
            {
                player.spriteRenderer.enabled = !player.spriteRenderer.enabled;
            }
            yield return new WaitForSeconds(0.1f);
        }

        player.spriteRenderer.enabled = true;
        player.isInvincible = false;
    }



    private void Die()
    {
        player.gameObject.SetActive(false);
        GameOverManager.Instance.ShowGameOver();
    }

    public IEnumerator RestoreHealth(int amount, float waitTime)
    {
        while (player.currentHealth < player.maxHealth)
        {
            if (player.currentHealth + amount > player.maxHealth)
            {
                player.currentHealth = player.maxHealth;
            }
            player.currentHealth += amount;
            player.NotifyHealthChanged();
            yield return new WaitForSeconds(waitTime);
        }
    }

}