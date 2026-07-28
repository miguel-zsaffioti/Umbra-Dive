using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveUntilAlign", story: "Move until is aligned to Player", category: "Action", id: "c2522e7783dbf24c2979fdcf1b7a5a70")]
public partial class MoveUntilAlignAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> AlignmentRange;
    private GrimReaper reaper;

    protected override Status OnStart()
    {
        reaper = GameObject.GetComponent<GrimReaper>();
        if (reaper == null) return Status.Failure;

        reaper.animator.SetBool("isMoving", true);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (reaper == null || reaper.player == null) return Status.Failure;

        if (IsAligned())
        {
            reaper.StopMovement();
            reaper.animator.SetBool("isMoving", false);
            return Status.Success;
        }

        float diffX = reaper.player.transform.position.x - reaper.rb.position.x;
        reaper.TryFlipTowards(diffX);

        float diffY = reaper.player.transform.position.y - reaper.rb.position.y;
        float distanceY = Mathf.Abs(diffY);

        float speedFactor = Mathf.Clamp01(distanceY / (AlignmentRange.Value * 2f));
        float effectiveSpeed = Speed.Value * Mathf.Max(speedFactor, 0.3f);

        Vector2 direction = new Vector2(0f, Mathf.Sign(diffY));
        reaper.SetMovement(direction, effectiveSpeed);

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (reaper != null) reaper.StopMovement();
    }

    private bool IsAligned()
    {
        float diffY = Mathf.Abs(reaper.transform.position.y - reaper.player.transform.position.y);
        return diffY <= AlignmentRange.Value;
    }
}