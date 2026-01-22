using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
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
        reaper = GameObject.GetComponent<GrimReaper>(); ;
        if (reaper == null) return Status.Failure;

        reaper.animator.SetBool("isMoving", true);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (reaper == null || reaper.player == null) return Status.Failure;

        Vector2 currentPos = reaper.transform.position;
        Vector2 targetPos = reaper.player.transform.position;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance <= StoppingDistance.Value)
        {
            reaper.animator.SetBool("isMoving", false);
            return Status.Success; // Chegou no alvo!
        }

        // 2. Lógica de Flip
        float diffX = targetPos.x - currentPos.x;
        if (Mathf.Abs(diffX) > 0.1f)
        {
            if (diffX > 0 && !reaper.isFacingRight) reaper.Flip();
            else if (diffX < 0 && reaper.isFacingRight) reaper.Flip();
        }

        Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, Speed.Value * Time.deltaTime);
        reaper.rb.MovePosition(newPos);

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

