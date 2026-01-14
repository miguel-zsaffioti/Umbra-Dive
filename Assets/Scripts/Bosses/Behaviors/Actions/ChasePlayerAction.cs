using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Player", story: "Chase Player with [Speed] speed", category: "Action", id: "e20f8ef9f1031ff202216d107b4d87b0")]
public partial class ChasePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Speed;
    private Boss reaper;

    protected override Status OnStart()
    {
        reaper = Boss.Instance;
        reaper.animator.SetBool("isMoving", true);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (reaper == null || reaper.player == null) return Status.Failure;
        Player player = reaper.player;

        Vector2 dir = (player.transform.position - reaper.transform.position);
        float distanceToPlayer = dir.magnitude;

        if (Mathf.Abs(dir.x) > 0.1f)
        {
            if (dir.x > 0 && !reaper.isFacingRight) reaper.Flip();
            else if (dir.x < 0 && reaper.isFacingRight) reaper.Flip();
        }

        Vector2 targetPosition = reaper.rb.position + (dir.normalized * Speed * Time.fixedDeltaTime);
        reaper.rb.MovePosition(targetPosition);

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

