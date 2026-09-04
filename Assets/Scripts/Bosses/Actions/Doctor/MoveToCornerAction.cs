using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToCorner", story: "Move to corner", category: "Action", id: "b5a063c08f05866cc4c2f2ca29ca1723")]
public partial class MoveToCornerAction : Action
{
    private Doctor doctor;
    private Vector2 targetPosition;

    [SerializeReference] public float moveSpeed = 5f;
    [SerializeReference] public float stoppingDistance = 0.3f;

    protected override Status OnStart()
    {
        doctor = GameObject.GetComponent<Doctor>();
        if (doctor == null || doctor.player == null) return Status.Failure;
        if (doctor.arenaCorners == null || doctor.arenaCorners.Count == 0) return Status.Failure;
        int i = UnityEngine.Random.Range(0, doctor.arenaCorners.Count);
        targetPosition = doctor.arenaCorners[i];
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
            //Debug.Log($"{currentPos} -> {targetPosition}, dist={dist}");
            return Status.Running;
        }

        doctor.StopMovement();
        doctor.rb.MovePosition(targetPosition);
        return Status.Success;
    }
}