public class BossHealthSystem
{
    private Boss boss;
    private FlashEffect flashEffect;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }        

    public BossHealthSystem(Boss boss, float maxHealth)
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

        boss.animator.SetTrigger("Die");
        boss.enabled = false;
    }
}