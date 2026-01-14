using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Normal Attack", story: "Normal Attack", category: "Action", id: "a22fd73f352bf842ae4bf8de0f2cf1b6")]
public partial class NormalAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Timer;
    [SerializeReference] public BlackboardVariable<float> Cooldown;

    protected override Status OnStart()
    {
        Timer.Value = Cooldown.Value;
        Boss.Instance.animator.SetTrigger("Attack");
        Debug.Log("Ataque");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

