using UnityEngine;

public class AttackState : PlayerState
{
    private float attackTimer;

    public AttackState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        animator.SetBool("isAttacking", true);
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
        rb.linearVelocity = inputSystem.MoveInput.normalized * player.moveSpeed;
    }
}
