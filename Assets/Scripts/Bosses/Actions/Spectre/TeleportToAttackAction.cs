using System;
using System.Collections;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Teleport to attack", story: "Teleport avoiding overlap", category: "Action", id: "e865466f179775d727d534ba204f4094")]
public partial class TeleportToAttackAction : SpectreMoveActionBase
{
    private float safeDistance = 1.5f;

    protected override Vector2 ComputeTarget(Spectre spectre)
    {
        if (spectre.player == null || spectre.player.rb == null) return spectre.rb.position;

        Vector2 currentPos = spectre.rb.position;
        Vector2 playerPos = spectre.player.rb.position;
        Vector2 target = currentPos;

        float distX = Mathf.Abs(currentPos.x - playerPos.x);
        float distY = Mathf.Abs(currentPos.y - playerPos.y);

        bool wantToAlignX = UnityEngine.Random.value < 0.5f;

        if (wantToAlignX && distY < safeDistance) wantToAlignX = false;
        else if (!wantToAlignX && distX < safeDistance) wantToAlignX = true;

        if (wantToAlignX) target.x = playerPos.x;
        else target.y = playerPos.y;

        return target;
    }

    protected override IEnumerator DoMovement(Spectre spectre, Vector2 target)
    {
        yield return spectre.TeleportTo(target, 1f);
    }
}