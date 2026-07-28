using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to special attack", story: "Move to special attack", category: "Action", id: "515bc6a3c143540436178f601cd369bc")]
public partial class MoveToSpecialAttackAction : SpectreMoveActionBase
{
    [SerializeReference] public BlackboardVariable<float> StoppingDistance;

    protected override Vector2 ComputeTarget(Spectre spectre)
    {
        return spectre.ClampToArena(spectre.player.transform.position);
    }

    protected override void UpdateTarget(Spectre spectre)
    {
        targetLocation = spectre.player.transform.position;
    }

    protected override Status OnUpdate()
    {
        UpdateTarget(spectre);

        float distanceToPlayer = Vector2.Distance(spectre.rb.position, targetLocation);

        if (distanceToPlayer <= StoppingDistance.Value)
        {
            spectre.StopMovement();
            return Status.Success;
        }

        Vector2 direction = (targetLocation - spectre.rb.position).normalized;
        spectre.SetMovement(direction, moveSpeed);

        return Status.Running;
    }
}