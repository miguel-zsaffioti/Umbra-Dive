using UnityEngine;

public class KnockbackState : PlayerState
{
    private float knockbackTimer;
    private bool bufferedAttack; 

    public KnockbackState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        knockbackTimer = player.knockbackDuration;
        bufferedAttack = false; 
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        knockbackTimer -= Time.deltaTime;

        if (inputSystem.AttackInput)
        {
            bufferedAttack = true;
        }

        if (knockbackTimer <= 0)
        {
            if (inputSystem.MoveInput == Vector2.zero)
            {
                rb.linearVelocity = Vector2.zero;
            }

            if (bufferedAttack)
            {
                stateMachine.ChangeState(player.attackState);
            }
            else
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

}