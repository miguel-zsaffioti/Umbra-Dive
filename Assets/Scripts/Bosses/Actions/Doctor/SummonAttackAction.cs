using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SummonAttack", story: "Summon attack", category: "Action", id: "4438093491084d1da001796d3cba5c71")]
public partial class SummonAttackAction : Action
{
    private Doctor doctor;

    protected override Status OnStart()
    {
        doctor = GameObject.GetComponent<Doctor>();
        if (doctor == null) return Status.Failure;

        doctor.QueueAttack(doctor.Summon);
        doctor.SetAttackDirection(Vector2.down);
        return Status.Success;
    }
}

