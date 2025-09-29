using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter()
    {
        enemy.animator.SetBool("isChasing", true);
    }

    public override void Exit()
    {
        enemy.animator.SetBool("isChasing", false);
    }

    public override void FrameUpdate()
    {
        if (!enemy.detectionSystem.IsPlayerDetected())
        {
            stateMachine.ChangeState(enemy.waitState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        Vector2 dir = (enemy.playerTransform.position - enemy.transform.position);

        if (dir.magnitude > enemy.data.stopDistance)
        {
            enemy.rb.AddForce(dir.normalized * enemy.data.moveSpeed, ForceMode2D.Force);

            if (enemy.rb.linearVelocity.magnitude > enemy.data.moveSpeed)
            {
                enemy.rb.linearVelocity = enemy.rb.linearVelocity.normalized * enemy.data.moveSpeed;
            }
        }
        else
        {
            enemy.rb.linearVelocity = Vector2.zero;
        }
    }
}
