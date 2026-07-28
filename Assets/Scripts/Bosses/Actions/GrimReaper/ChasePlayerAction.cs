using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Player", story: "Chase Player with [Speed]", category: "Action", id: "e20f8ef9f1031ff202216d107b4d87b0")]
public partial class ChasePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> StoppingDistance;

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

        Vector2 currentPos = reaper.rb.position;
        Vector2 targetPos = reaper.player.transform.position;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance <= StoppingDistance.Value)
        {
            reaper.StopMovement();
            reaper.animator.SetBool("isMoving", false);
            return Status.Success;
        }

        reaper.TryFlipTowards(targetPos.x - currentPos.x);

        float speedFactor = Mathf.Clamp01(distance / (StoppingDistance.Value * 2f));
        float effectiveSpeed = Speed.Value * Mathf.Max(speedFactor, 0.3f);

        Vector2 direction = targetPos - currentPos;
        reaper.SetMovement(direction, effectiveSpeed);

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (reaper != null) reaper.StopMovement();
    }
}