using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    public override void Enter()
    {
        animator.SetBool("isWalking", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (inputSystem.AttackInput)
        {
            stateMachine.ChangeState(player.attackState);
            return;
        }
        if (inputSystem.RollInput)
        {
            stateMachine.ChangeState(player.rollState);
            return;
        }
        if (inputSystem.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(player.walkState);
        }
    }

    public override void PhysicsUpdate()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
