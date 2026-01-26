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

        if (isAligned())
        {
            reaper.animator.SetBool("isMoving", false);
            return Status.Success;
        }

        float diffX = reaper.player.transform.position.x - reaper.transform.position.x;
        if (Mathf.Abs(diffX) > 0.5f) 
        {
            if (diffX > 0 && !reaper.isFacingRight) reaper.Flip();
            else if (diffX < 0 && reaper.isFacingRight) reaper.Flip();
        }

        float newY = Mathf.MoveTowards(
            reaper.rb.position.y,
            reaper.player.transform.position.y,
            Speed.Value * Time.deltaTime
        );

        Vector2 targetPosition = new Vector2(reaper.rb.position.x, newY);
        reaper.rb.MovePosition(targetPosition);

        return Status.Running;
    }

    private bool isAligned() {
        float diffY = Mathf.Abs(reaper.transform.position.y - reaper.player.transform.position.y);
        return diffY <= AlignmentRange.Value;
    }
}

