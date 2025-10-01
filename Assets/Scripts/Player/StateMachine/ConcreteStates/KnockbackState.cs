using UnityEngine;

public class KnockbackState : PlayerState
{
    private float knockbackTimer;

    public KnockbackState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        knockbackTimer = player.knockbackDuration;
    }

    public override void FrameUpdate()
    {
        knockbackTimer -= Time.deltaTime;
        if (knockbackTimer <= 0)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }
}
