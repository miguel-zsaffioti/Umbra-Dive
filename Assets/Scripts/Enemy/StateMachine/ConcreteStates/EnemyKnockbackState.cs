using UnityEngine;

public class EnemyKnockbackState : EnemyState
{
    private float timer;

    public EnemyKnockbackState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter()
    {
        timer = enemy.data.knockbackStunDuration;

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