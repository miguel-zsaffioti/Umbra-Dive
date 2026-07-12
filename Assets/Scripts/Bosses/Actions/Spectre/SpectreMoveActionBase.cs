using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;

public abstract partial class SpectreMoveActionBase : Action
{
    protected Spectre spectre;
    protected Vector2 targetLocation;
    private bool isMoving = false;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();
        if (spectre == null) return Status.Failure;

        targetLocation = ComputeTarget(spectre);

        isMoving = true;
        spectre.StartCoroutine(RunMovement());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return isMoving ? Status.Running : Status.Success;
    }

    private IEnumerator RunMovement()
    {
        yield return DoMovement(spectre, targetLocation);
        isMoving = false;
    }

    protected abstract Vector2 ComputeTarget(Spectre spectre);

    protected virtual IEnumerator DoMovement(Spectre spectre, Vector2 target)
    {
        yield return spectre.MoveToCoroutine(target, 5f);
    }
}