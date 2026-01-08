using UnityEngine;

public class BlockState : PlayerState
{
    public BlockState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        player.animator.Play("Empty", 1, 0f);
        player.EnableCombatLayer();
        animator.SetBool("isBlocking", true);
    }

    public override void Exit()
    {
        player.animator.SetLayerWeight(1, 0f);
        animator.SetBool("isBlocking", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!inputSystem.BlockInput)
        {
            if (inputSystem.MoveInput.magnitude > 0.1f)
            {
                stateMachine.ChangeState(player.walkState);
            }
            else
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    public override void HandleAnimation()
    {
        Vector2 moveInput = inputSystem.MoveInput;

        animator.SetFloat("XInput", moveInput.x);
        animator.SetFloat("YInput", moveInput.y);

        if (moveInput.magnitude > 0.1f)
        {
            animator.SetFloat("LastX", moveInput.x);
            animator.SetFloat("LastY", moveInput.y);
        }
    }

    public override void PhysicsUpdate()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
