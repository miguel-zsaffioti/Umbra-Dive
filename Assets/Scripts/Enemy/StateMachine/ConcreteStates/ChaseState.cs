using UnityEngine;

public class ChaseState : EnemyState
{
    private float repathTimer;
    private const float RepathInterval = 0.2f;

    public ChaseState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter()
    {
        enemy.animator.SetBool("isChasing", true);
        repathTimer = 0f;

        if (enemy.agent != null)
        {
            enemy.agent.nextPosition = enemy.transform.position;
        }
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

        if (enemy.data.leashRange > 0f)
        {
            float distanceFromSpawn = Vector2.Distance(enemy.transform.position, enemy.spawnPosition);
            if (distanceFromSpawn > enemy.data.leashRange)
            {
                stateMachine.ChangeState(enemy.waitState);
                return;
            }
        }
    }

    public override void PhysicsUpdate()
    {
        if (enemy.playerTransform == null) return;

        Vector2 straightDir = (Vector2)enemy.playerTransform.position - enemy.rb.position;
        float distanceToPlayer = straightDir.magnitude;

        if (Mathf.Abs(straightDir.x) > 0.2f)
        {
            enemy.FaceDirection(straightDir);
        }

        if (distanceToPlayer <= enemy.data.attackDistance && enemy.TryAttack())
        {
            return;
        }
        else if (distanceToPlayer > enemy.data.stopDistance)
        {
            Vector2 targetPosition;

            if (enemy.agent != null)
            {
                repathTimer -= Time.fixedDeltaTime;
                if (repathTimer <= 0f)
                {
                    enemy.agent.SetDestination(enemy.playerTransform.position);
                    repathTimer = RepathInterval;
                }
                Vector2 pathDir = ((Vector2)enemy.agent.desiredVelocity).normalized;
                if (pathDir.sqrMagnitude < 0.001f)
                {
                    pathDir = straightDir.normalized;
                }

                targetPosition = enemy.rb.position + pathDir * enemy.data.moveSpeed * Time.fixedDeltaTime;
            }
            else
            {
                targetPosition = enemy.rb.position + straightDir.normalized * enemy.data.moveSpeed * Time.fixedDeltaTime;
            }

            enemy.rb.MovePosition(targetPosition);

            if (enemy.agent != null)
            {
                enemy.agent.nextPosition = targetPosition;
            }
        }
        else
        {
            enemy.rb.linearVelocity = Vector2.zero;
        }
    }
}