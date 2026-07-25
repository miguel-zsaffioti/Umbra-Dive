using System.Diagnostics;

public class BossHealthSystem
{
    private Boss boss;
    private FlashEffect flashEffect;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public BossHealthSystem(Boss boss, int maxHealth)
    {
        this.boss = boss;
        this.flashEffect = boss.GetComponent<FlashEffect>();
        CurrentHealth = maxHealth;
        IsDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        CurrentHealth -= damage;
        boss.NotifyHealthChanged();

        if (CurrentHealth <= 0)
        {
            StartDying();
        }
        else
        {
            flashEffect.Flash();
        }
    }

    private void StartDying()
    {
        IsDead = true;
        boss.rb.simulated = false;
        boss.StopAllCoroutines();

        if (boss.behaviorAgent != null)
        {
            boss.behaviorAgent.enabled = false;
        }

        boss.Die();
        boss.enabled = false;
    }
}