using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;

    protected Animator animator;
    protected Rigidbody2D rb;
    protected PlayerInputSystem inputSystem;

    public PlayerState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;

        animator = player.animator;
        rb = player.rb;
        inputSystem = player.inputSystem;
        
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void FrameUpdate() { HandleAnimation(); }
    public virtual void PhysicsUpdate() { }
    public virtual void HandleAnimation() { }

}
