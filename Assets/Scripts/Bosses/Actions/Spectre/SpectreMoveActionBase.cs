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

        //Debug.Log($"Frame={Time.frameCount} currentPos={currentPos} target={targetLocation} distX={distX}");

        if (distX > stoppingDistance)
        {
            float dirX = Mathf.Sign(targetLocation.x - currentPos.x);
            spectre.SetMovement(new Vector2(dirX, 0f), 0f);

            float newX = Mathf.MoveTowards(currentPos.x, targetLocation.x, step);
            spectre.rb.MovePosition(new Vector2(newX, currentPos.y));
            return Status.Running;
        }

        if (distY > stoppingDistance)
        {
            float dirY = Mathf.Sign(targetLocation.y - currentPos.y);
            spectre.SetMovement(new Vector2(0f, dirY), 0f); 

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