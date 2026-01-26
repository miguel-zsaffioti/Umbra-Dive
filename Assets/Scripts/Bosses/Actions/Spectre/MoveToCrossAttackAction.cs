using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using JetBrains.Annotations;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to cross attack", story: "Move to cross attack", category: "Action", id: "26a771c9f5a922bae9135f4a84876f0b")]
public partial class MoveToCrossAttackAction : Action
{

    private Spectre spectre;
    private Vector2 targetLocation;
    private bool isMoving = false;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();
        if (spectre == null) return Status.Failure;

        Vector2 currentPos = spectre.rb.position;
        Vector2 playerPos = spectre.player.rb.position; 
        targetLocation = currentPos;

        float distX = Mathf.Abs(currentPos.x - playerPos.x);
        float distY = Mathf.Abs(currentPos.y - playerPos.y);

        if (distX > distY)
        {
            targetLocation.y = playerPos.y; 
        }
        else
        {
            targetLocation.x = playerPos.x; 
        }

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
        yield return spectre.MoveToCoroutine(targetLocation, 5f); 
        isMoving = false;
    }
}

