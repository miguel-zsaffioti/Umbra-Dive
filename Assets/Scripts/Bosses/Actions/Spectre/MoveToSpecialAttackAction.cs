using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to special attack", story: "Move to special attack", category: "Action", id: "515bc6a3c143540436178f601cd369bc")]
public partial class MoveToSpecialAttackAction : Action
{
    private Spectre spectre;
    [SerializeReference] public BlackboardVariable<float> StoppingDistance;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>(); ;
        if (spectre == null) return Status.Failure;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (spectre == null || spectre.player == null) return Status.Failure;

        Vector2 currentPos = spectre.transform.position;
        Vector2 targetPos = spectre.player.transform.position;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance <= StoppingDistance.Value)
        {
            return Status.Success; // Chegou no alvo!
        }

        Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, 5f * Time.deltaTime);
        spectre.rb.MovePosition(newPos);

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

