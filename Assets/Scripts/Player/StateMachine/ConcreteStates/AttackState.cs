using UnityEngine;

public class AttackState : PlayerState
{
    private float attackTimer;
    private Vector2 attackDirection;

    public AttackState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        animator.SetBool("isAttacking", true);
        if (inputSystem.MoveInput.magnitude > 0.1f)
        {
            attackDirection = inputSystem.MoveInput.normalized;
        }
        else
        {
            attackDirection = new Vector2(animator.GetFloat("LastX"), animator.GetFloat("LastY"));
        }

        attackTimer = player.attackAnimation.length;
    }

    public override void Exit()
    {
        animator.SetBool("isAttacking", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            if (inputSystem.MoveInput == Vector2.zero)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.walkState);
            }
        }

    }

    public override void PhysicsUpdate()
    {
        rb.linearVelocity = attackDirection * player.attackMoveSpeed;
    }
}
