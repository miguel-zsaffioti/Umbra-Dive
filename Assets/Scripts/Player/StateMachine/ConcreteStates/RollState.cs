using UnityEngine;
using UnityEngine.InputSystem;

public class RollState : PlayerState
{
    private Vector2 rollDirection;
    private float rollTimer;
    private bool bufferedRoll;

    public RollState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        animator.SetBool("isRolling", true);
        bufferedRoll = false;
        if (inputSystem.MoveInput.magnitude > 0.1f)
        {
            rollDirection = inputSystem.MoveInput.normalized;
        }
        else
        {
            rollDirection = new Vector2(animator.GetFloat("LastX"), animator.GetFloat("LastY"));
        }

        rollTimer = player.rollAnimation.length;

        rb.AddForce(rollDirection * player.rollForce, ForceMode2D.Impulse);
        player.isInvincible = true;
        player.rollCooldown.Reset();
    }

    public override void Exit()
    {
        animator.SetBool("isRolling", false);
        rb.linearVelocity = Vector2.zero;
        player.isInvincible = false;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (inputSystem.AttackInput) 
        {
            bufferedRoll = true;
        }
        if (rollTimer > 0)
        {
            rollTimer -= Time.deltaTime;
        }
        else
        {
            if (bufferedRoll) 
            {
                stateMachine.ChangeState(player.attackState);
            }
            else if (inputSystem.MoveInput == Vector2.zero)
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
