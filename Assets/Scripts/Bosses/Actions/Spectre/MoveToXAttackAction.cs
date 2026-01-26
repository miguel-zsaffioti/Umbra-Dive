using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to X attack", story: "Move to X attack", category: "Action", id: "d9cf21cb1775a72e2d65af18fe3fc37d")]
public partial class MoveToDiagonalSafeAction : Action
{
    private Spectre spectre;
    private Vector2 targetLocation;
    private bool isMoving = false;

    [SerializeReference] public BlackboardVariable<float> minRange;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();
        if (spectre == null) return Status.Failure;

        Vector2 currentPos = spectre.rb.position;
        Vector2 playerPos = spectre.player.rb.position;

        // 1. Pega as distâncias atuais
        float distX = Mathf.Abs(currentPos.x - playerPos.x);
        float distY = Mathf.Abs(currentPos.y - playerPos.y);

        float dirX = (currentPos.x > playerPos.x) ? 1f : -1f;
        float dirY = (currentPos.y > playerPos.y) ? 1f : -1f;

        float targetDist = Mathf.Max(Mathf.Min(distX, distY), minRange.Value);

        targetLocation = new Vector2(
            playerPos.x + (targetDist * dirX),
            playerPos.y + (targetDist * dirY)
        );

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