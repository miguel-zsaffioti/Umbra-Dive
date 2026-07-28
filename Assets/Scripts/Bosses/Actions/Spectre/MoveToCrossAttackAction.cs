using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to cross attack", story: "Move to cross attack", category: "Action", id: "26a771c9f5a922bae9135f4a84876f0b")]
public partial class MoveToCrossAttackAction : SpectreMoveActionBase
{
    protected override Vector2 ComputeTarget(Spectre spectre)
    {
        Vector2 currentPos = spectre.rb.position;
        Vector2 playerPos = spectre.player.rb.position;
        Vector2 target = currentPos;

        float distX = Mathf.Abs(currentPos.x - playerPos.x);
        float distY = Mathf.Abs(currentPos.y - playerPos.y);

        if (distX > distY)
            target.y = playerPos.y;
        else
            target.x = playerPos.x;

        return spectre.ClampToArena(target);
    }
}