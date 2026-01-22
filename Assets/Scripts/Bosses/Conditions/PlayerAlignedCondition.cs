using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Player Aligned", story: "Player in [AlignmentRange]", category: "Conditions", id: "f3ade51fa67a1a80544c4d672b4cf191")]
public partial class PlayerAlignedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> AlignmentRange;
    [SerializeReference] public BlackboardVariable<float> Timer;

    public override bool IsTrue()
    {
        Boss reaper = Boss.Instance;

        if (reaper == null || reaper.player == null) return false;

        float diffY = Mathf.Abs(reaper.transform.position.y - reaper.player.transform.position.y);
        return diffY <= AlignmentRange.Value;
    }

    public override void OnStart()
    {
        IsTrue();
    }

}
