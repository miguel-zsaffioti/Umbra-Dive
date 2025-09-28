using UnityEngine;
using UnityEngine.InputSystem;

public class RollState : PlayerState
{
    private Vector2 rollDirection;
    private float rollTimer;

    public RollState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        animator.SetBool("isRolling", true);
        if(inputSystem.MoveInput.magnitude > 0.1f)
        {
            rollDirection = inputSystem.MoveInput.normalized;
        }
        else
        {
            rollDirection = new Vector2(animator.GetFloat("LastX"), animator.GetFloat("LastY"));
        }

        rollTimer = player.rollAnimation.length;

        rb.AddForce(rollDirection * player.rollForce, ForceMode2D.Impulse);
    }

    public override void Exit()
    {
        animator.SetBool("isRolling", false);
        rb.linearVelocity = Vector2.zero;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (rollTimer > 0)
        {
            rollTimer -= Time.deltaTime;
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

}
