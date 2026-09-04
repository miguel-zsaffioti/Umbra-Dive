using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ProjectileAttack", story: "Projectile attack", category: "Action", id: "e9150aeae823bde404173e65f8caad56")]
public partial class ProjectileAttackAction : Action
{
    private Doctor doctor;

    protected override Status OnStart()
    {
        doctor = GameObject.GetComponent<Doctor>();
        if (doctor == null) return Status.Failure;

        Vector2 nearestCorner = doctor.GetNearestCorner(out int cornerIndex);
        Vector2 animationDir = Vector2.zero;
        switch (cornerIndex)
        {
            case 0: animationDir = Vector2.right; break;
            case 1: animationDir = Vector2.left; break;
            case 2: animationDir = Vector2.left; break;
            case 3: animationDir = Vector2.right; break;
        }

        doctor.QueueAttack(doctor.Projectile);
        doctor.SetAttackDirection(animationDir);

        return Status.Success;
    }
}

