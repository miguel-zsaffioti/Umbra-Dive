using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CrossAttack", story: "Cross attack", category: "Action", id: "590cca23e6de2742733821d828df171b")]
public partial class CrossAttackAction : Action
{
    private Spectre spectre;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();
        if (spectre == null) return Status.Failure;

        spectre.SpawnProjectileInCrossPattern();
        return Status.Success;
    }
}

