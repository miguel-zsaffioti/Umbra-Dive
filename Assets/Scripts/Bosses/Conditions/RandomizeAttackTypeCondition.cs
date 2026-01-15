using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "RandomizeAttackType", story: "Number is less than [Chance]", category: "Conditions", id: "1245e6582d85a15535256d3fe03dd48b")]
public partial class RandomizeAttackTypeCondition : Condition
{

    [SerializeReference] public BlackboardVariable<float> Chance;

    public override bool IsTrue()
    {
        return UnityEngine.Random.value < Chance * 0.01f;
    }

    public override void OnStart()
    {
        IsTrue();
    }

    public override void OnEnd()
    {
    }
}
