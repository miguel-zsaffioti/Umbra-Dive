using System;
using System.Collections;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to special attack", story: "Move to special attack", category: "Action", id: "515bc6a3c143540436178f601cd369bc")]
public partial class MoveToSpecialAttackAction : SpectreMoveActionBase
{
    [SerializeReference] public BlackboardVariable<float> StoppingDistance;

    protected override Vector2 ComputeTarget(Spectre spectre)
    {
        return spectre.player.transform.position; // alvo é a posição atual do player
    }

    protected override IEnumerator DoMovement(Spectre spectre, Vector2 target)
    {
        while (Vector2.Distance(spectre.transform.position, spectre.player.transform.position) > StoppingDistance.Value)
        {
            Vector2 currentPos = spectre.rb.position;
            Vector2 newPos = Vector2.MoveTowards(currentPos, spectre.player.transform.position, 5f * Time.deltaTime);
            spectre.rb.MovePosition(newPos);
            yield return null;
        }
    }
}

