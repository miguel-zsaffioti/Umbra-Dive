using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Teleport attack", story: "Teleport attack", category: "Action", id: "a3c96e5d4512743c746fec3b1dfc153f")]
public partial class TeleportAttackAction : Action
{
    private Spectre spectre;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();
   
        if (spectre == null) return Status.Failure;
        spectre.SpawnProjectileTeleport();

        return Status.Success;
    }
}

