using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;

public abstract partial class SpectreMoveActionBase : Action
{
    protected Spectre spectre;
    protected Vector2 targetLocation;
    protected float moveSpeed = 5f;
    protected float stoppingDistance = 0.05f;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();
        if (spectre == null) return Status.Failure;

        targetLocation = ComputeTarget(spectre);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        UpdateTarget(spectre);

        Vector2 currentPos = spectre.rb.position;
        float step = moveSpeed * Time.fixedDeltaTime;

        float distX = Mathf.Abs(targetLocation.x - currentPos.x);
        float distY = Mathf.Abs(targetLocation.y - currentPos.y);


        if (distX > stoppingDistance)
        {
            float newX = Mathf.MoveTowards(currentPos.x, targetLocation.x, step);
            spectre.rb.MovePosition(new Vector2(newX, currentPos.y));
            return Status.Running;
        }

        if (distY > stoppingDistance)
        {
            float newY = Mathf.MoveTowards(currentPos.y, targetLocation.y, step);
            spectre.rb.MovePosition(new Vector2(targetLocation.x, newY));
            return Status.Running;
        }

        spectre.StopMovement();
        spectre.rb.MovePosition(targetLocation);
        return Status.Success;
    }

    protected override void OnEnd()
    {
        if (spectre != null) spectre.StopMovement();
    }

    protected abstract Vector2 ComputeTarget(Spectre spectre);

    protected virtual void UpdateTarget(Spectre spectre) { }
}