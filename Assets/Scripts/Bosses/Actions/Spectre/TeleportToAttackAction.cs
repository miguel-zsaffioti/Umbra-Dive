using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Teleport to attack", story: "Teleport avoiding overlap", category: "Action", id: "e865466f179775d727d534ba204f4094")]
public partial class TeleportToAttackAction : Action
{
    private Spectre spectre;
    private Vector2 targetLocation;
    private bool isTeleporting = false;

    private float safeDistance = 1.5f;

    protected override Status OnStart()
    {
        spectre = GameObject.GetComponent<Spectre>();


        if (spectre == null || spectre.player == null || spectre.player.rb == null)
        {
            return Status.Failure;
        }

        Vector2 currentPos = spectre.rb.position;
        Vector2 playerPos = spectre.player.rb.position;
        targetLocation = currentPos;

        float distX = Mathf.Abs(currentPos.x - playerPos.x);
        float distY = Mathf.Abs(currentPos.y - playerPos.y);

        bool wantToAlignX = UnityEngine.Random.value < 0.5f;

    

        if (wantToAlignX)
        {
            if (distY < safeDistance)
            {
                wantToAlignX = false;
            }
        }
        else 
        {
            if (distX < safeDistance)
            {
                wantToAlignX = true;
            }
        }
        if (wantToAlignX)
        {
            targetLocation.x = playerPos.x;
        }
        else
        {
            targetLocation.y = playerPos.y;
        }

        isTeleporting = true;
        spectre.StartCoroutine(RunTeleport());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return isTeleporting ? Status.Running : Status.Success;
    }

    private IEnumerator RunTeleport()
    {
        yield return spectre.TeleportTo(targetLocation, 1f);
        isTeleporting = false;
    }
}