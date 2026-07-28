using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "XAttack", story: "X attack", category: "Action", id: "e0d7973cb474795cd31c282660d99fbe")]
public partial class XAttackAction : Action
{
    private Spectre spectre;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();
        if (spectre == null) return Status.Failure;

        spectre.SpawnProjectileInXPattern();
        return Status.Success;
    }
}

