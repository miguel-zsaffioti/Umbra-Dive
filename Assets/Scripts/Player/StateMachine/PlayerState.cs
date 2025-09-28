using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;

    public PlayerState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }

    public virtual float HandleInput()
    {
        float raw = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(raw) < 0.01f)
            return 0f;
        return Mathf.Sign(raw);
    }

}
