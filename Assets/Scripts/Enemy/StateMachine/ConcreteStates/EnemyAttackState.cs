using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private Vector2 attackDirection;
    private bool isDashing;

    public EnemyAttackState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter()
    {
        isDashing = false;
        enemy.rb.linearVelocity = Vector2.zero;
        attackDirection = (enemy.playerTransform.position - enemy.transform.position).normalized;

        enemy.FaceDirection(attackDirection);

        enemy.animator.SetTrigger("Attack");
    }

    public override void PhysicsUpdate()
    {
        if (isDashing)
        {
            enemy.rb.linearVelocity = attackDirection * enemy.data.attackDashSpeed;
        }
    }

    public override void OnAnimationTrigger(string eventName)
    {
        if (eventName == "StartDash")
        {
            isDashing = true;
        }
        else if (eventName == "FinishAttack")
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void Exit()
    {
        enemy.rb.linearVelocity = Vector2.zero;
        enemy.StartCoroutine(((Bat)enemy).waitToAttack());
    }

    public override void FrameUpdate() { }
}