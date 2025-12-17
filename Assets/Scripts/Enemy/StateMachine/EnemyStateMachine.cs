using UnityEngine;

public class EnemyStateMachine
{
    private EnemyState currState { get; set; }

    public void Start(EnemyState state)
    {
        currState = state;
        currState.Enter();
    }

    public void ChangeState(EnemyState state)
    {
        currState.Exit();
        currState = state;
        currState.Enter();
    }

    public void FrameUpdate()
    {
        currState.FrameUpdate();
    }

    public void PhysicsUpdate()
    {
        currState.PhysicsUpdate();
    }
    public void OnAnimationTrigger(string eventName)
    {
        currState.OnAnimationTrigger(eventName);
    }
}
