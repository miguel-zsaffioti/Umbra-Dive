using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToCenter", story: "Move center to attack", category: "Action", id: "42cffd2574651de353b55cdbdfa28494")]
public partial class MoveToCenterAction : Action
{

    private Doctor doctor;
    private Vector2 targetPosition;

    [SerializeReference] public float moveSpeed = 5f;
    [SerializeReference] public float stoppingDistance = 0.2f;

    protected override Status OnStart()
    {
        doctor = GameObject.GetComponent<Doctor>();
        if (doctor == null || doctor.player == null) return Status.Failure;
        targetPosition = doctor.arenaCenter;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector2 currentPos = doctor.rb.position;
        float dist = Vector2.Distance(currentPos, targetPosition);

        if (dist > stoppingDistance)
        {
            Vector2 dir = (targetPosition - currentPos).normalized;
            doctor.SetMovement(dir, moveSpeed);
            doctor.rb.position = doctor.ClampToArena(doctor.rb.position);
            return Status.Running;
        }

        doctor.StopMovement();
        doctor.rb.MovePosition(targetPosition);
        return Status.Success;
    }

}

