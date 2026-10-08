using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToFrontAttack", story: "Move to front attack position", category: "Action", id: "39e9356e4e3c81129487daebd8fe73ce")]
public partial class MoveToFrontAttackAction : Action
{
    private Doctor doctor;
    private Vector2 targetPosition;

    [SerializeReference] public float moveSpeed = 5f;
    [SerializeReference] public float stoppingDistance = 0.2f;
    [SerializeReference] public float attackDistance = 3f;

    protected override Status OnStart()
    {
        doctor = GameObject.GetComponent<Doctor>();
        if (doctor == null || doctor.player == null) return Status.Failure;

        UpdateTarget();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        UpdateTarget();

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

    private void UpdateTarget()
    {
        Vector2 playerPos = doctor.player.rb.position;
        Vector2 currentPos = doctor.rb.position;
        Vector2 diff = currentPos - playerPos;

        Vector2 cardinalDir = Mathf.Abs(diff.x) > Mathf.Abs(diff.y)
            ? new Vector2(Mathf.Sign(diff.x), 0f)
            : new Vector2(0f, Mathf.Sign(diff.y));

        targetPosition = doctor.ClampToArena(playerPos + cardinalDir * attackDistance);
    }

    protected override void OnEnd()
    {
        doctor?.StopMovement();
    }
}