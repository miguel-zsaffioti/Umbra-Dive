using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Player in Range", story: "Player in [Range]", category: "Conditions", id: "6d911381af4803e80757e7805fe55609")]
public partial class PlayerInRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> Range;

    public override bool IsTrue()
    {
        Boss boss = Boss.Instance;
        if (boss == null) return false;

        return boss.GetPlayerInRange(Range) != null;
    }

    public override void OnStart()
    {
       IsTrue();
    }

}
