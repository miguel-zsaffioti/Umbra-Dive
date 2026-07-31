using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Teleport to attack", story: "Teleport avoiding overlap", category: "Action", id: "e865466f179775d727d534ba204f4094")]
public partial class TeleportToAttackAction : Action
{
    private Spectre spectre;
    private float safeDistance = 1.5f;
    private float waitTime = 1f;
    private float timer = 0f;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();
        if (spectre == null) return Status.Failure;

        spectre.StopMovement();
        timer = 0f;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= waitTime)
        {
            Vector2 target = ComputeTarget(spectre);

            spectre.PlayTeleportEffect(spectre.rb.position);

            spectre.rb.position = target;
            spectre.PlayTeleportEffect(target);

            return Status.Success;
        }

        return Status.Running;
    }

    private Vector2 ComputeTarget(Spectre spectre)
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

        return spectre.ClampToArena(target);
    }
}