using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AreaSingleAttack", story: "Attack in close range", category: "Action", id: "7e1495d56878d129689ad8fd37b5fe5f")]
public partial class AreaSingleAttackAction : Action
{
    private Doctor doctor;

    protected override Status OnStart()
    {
        doctor = GameObject.GetComponent<Doctor>();
        if (doctor == null) return Status.Failure;

        doctor.QueueAttack(doctor.AreaAttack);
        doctor.SetAttackDirection(Vector2.down);
        return Status.Success;
    }
}

