using UnityEngine;
using UnityEngine.InputSystem;

public class WalkState : PlayerState
{
    public WalkState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    public override void Enter()
    {
        animator.SetBool("isWalking", true);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (inputSystem.RollInput)
        {
            stateMachine.ChangeState(player.rollState);
            return;
        }
        if (inputSystem.MoveInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.idleState);
            return;
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
        rb.linearVelocity = inputSystem.MoveInput.normalized * player.moveSpeed;
    }
}
