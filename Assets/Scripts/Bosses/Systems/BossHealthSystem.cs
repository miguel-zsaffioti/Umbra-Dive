using UnityEngine;

public class BossHealthSystem {

    Boss boss;
    private FlashEffect flashEffect;

    public BossHealthSystem(Boss boss)
    {
        this.boss = boss;
        this.flashEffect = boss.GetComponent<FlashEffect>();
    }

     public void TakeDamage(int damage)
    {
        if (boss.isDead) return;

        boss.currentHealth -= damage;

        if (boss.currentHealth <= 0)
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
        boss.isDead = true;

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
    

