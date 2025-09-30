using UnityEditor;
using UnityEngine;

public class EnemyHealthSystem
{
    Enemy enemy;

    public EnemyHealthSystem(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void TakeDamage(int damage)
    {
        enemy.currentHealth -= damage;


        if (enemy.currentHealth <= 0)
        {
            StartDying();
            return;
        }
        enemy.animator.SetTrigger("Hurt");
    }

    private void StartDying()
    {

        enemy.animator.SetTrigger("Die");
    }

}
