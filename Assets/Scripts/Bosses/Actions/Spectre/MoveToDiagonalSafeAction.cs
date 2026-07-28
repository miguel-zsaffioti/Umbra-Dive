using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to X attack", story: "Move to X attack", category: "Action", id: "d9cf21cb1775a72e2d65af18fe3fc37d")]
public partial class MoveToDiagonalSafeAction : SpectreMoveActionBase
{
    [SerializeReference] public BlackboardVariable<float> minRange;

    protected override Vector2 ComputeTarget(Spectre spectre)
    {
        Vector2 currentPos = spectre.rb.position;
        Vector2 playerPos = spectre.player.rb.position;

        float distX = Mathf.Abs(currentPos.x - playerPos.x);
        float distY = Mathf.Abs(currentPos.y - playerPos.y);

        float dirX = (currentPos.x > playerPos.x) ? 1f : -1f;
        float dirY = (currentPos.y > playerPos.y) ? 1f : -1f;

        float targetDist = Mathf.Max(Mathf.Min(distX, distY), minRange.Value);

        Vector2 target = new Vector2(
            playerPos.x + (targetDist * dirX),
            playerPos.y + (targetDist * dirY)
        );

        return spectre.ClampToArena(target);
    }
}