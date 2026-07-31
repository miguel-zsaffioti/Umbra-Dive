using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Attack Available", story: "[AttackId] not used in last [Cooldown] attacks", category: "Conditions", id: "7b3f2a9c1d4e4f6a8b0c2d3e4f5a6b7c")]
public partial class AttackAvailableCondition : Condition
{
    [SerializeReference] public BlackboardVariable<string> AttackId;
    [SerializeReference] public BlackboardVariable<int> Cooldown;

    public override bool IsTrue()
    {
        Boss boss = Boss.Instance;
        if (boss == null) return true;

        return boss.CanUseAttack(AttackId.Value, Cooldown.Value);
    }
}