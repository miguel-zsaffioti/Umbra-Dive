using UnityEngine;

public class WaitState : EnemyState
{
    public WaitState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void FrameUpdate()
    {
        if (enemy.detectionSystem.IsPlayerDetected())
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void PhysicsUpdate()
    {
        enemy.rb.linearVelocity = Vector2.zero;
    }
}
