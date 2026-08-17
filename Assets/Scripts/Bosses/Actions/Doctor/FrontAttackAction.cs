using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FrontAttack", story: "Attack towards player", category: "Action", id: "c875aa4209c89bc4787dd807cd7f4b48")]
public partial class FrontAttackAction : Action
{
    private Doctor doctor;

    protected override Status OnStart()
    {
        doctor = GameObject.GetComponent<Doctor>();
        if (doctor == null || doctor.player == null) return Status.Failure;

        Vector2 diff = doctor.player.rb.position - doctor.rb.position;
        Vector2 attackDir = Mathf.Abs(diff.x) > Mathf.Abs(diff.y)
            ? new Vector2(Mathf.Sign(diff.x), 0f)
            : new Vector2(0f, Mathf.Sign(diff.y));

        doctor.frontAttackDirection = attackDir;
        doctor.FrontAttack();
        return Status.Success;
    }
}

