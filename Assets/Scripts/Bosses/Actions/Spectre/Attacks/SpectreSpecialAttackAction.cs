using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Spectre special attack", story: "Spectre special attack", category: "Action", id: "0473f50aa44a9e30b964e58c6647450c")]
public partial class SpectreSpecialAttackAction : Action
{
    private Spectre spectre;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();
        if (spectre == null) return Status.Failure;

        spectre.SpawnProjectileInCrossPattern();
        spectre.SpawnProjectileInXPattern();
        return Status.Success;
    }
}

