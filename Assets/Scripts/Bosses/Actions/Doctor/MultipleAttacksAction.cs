using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MultipleAttacks", story: "Multiple attacks in arena", category: "Action", id: "262d4212096edda144a597f54f46c12b")]
public partial class MultipleAttacksAction : Action
{
    private Doctor doctor;
    protected override Status OnStart()
    {
        doctor = GameObject.GetComponent<Doctor>();
        if (doctor == null) return Status.Failure;

        doctor.QueueAttack(doctor.MultipleAreaAttack);
        doctor.SetAttackDirection(Vector2.down);
        return Status.Success;
    }
}

