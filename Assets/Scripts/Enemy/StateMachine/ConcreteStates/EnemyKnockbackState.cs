using UnityEngine;

public class EnemyKnockbackState : EnemyState
{
    private float timer;

    public EnemyKnockbackState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter()
    {
        // Define quanto tempo o inimigo fica "atordoado" voando para trás
        // Você pode criar uma variável 'knockbackDuration' no Enemy.cs ou usar um valor fixo aqui
        timer = 0.2f;

        // Opcional: Tocar animação de dano
        enemy.animator.SetTrigger("Hurt");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (enemy.detectionSystem.IsPlayerDetected())
            {
                stateMachine.ChangeState(enemy.chaseState);
            }
            else
            {
                stateMachine.ChangeState(enemy.waitState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
