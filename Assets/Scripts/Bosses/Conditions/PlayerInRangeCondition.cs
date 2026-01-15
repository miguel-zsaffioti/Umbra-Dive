using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Player in Range", story: "Player in [Range] and [Timer] equals 0", category: "Conditions", id: "6d911381af4803e80757e7805fe55609")]
public partial class PlayerInRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<float> Timer;

    public override bool IsTrue()
    {
        Boss boss = Boss.Instance;
        Collider2D collision = Physics2D.OverlapCircle(boss.transform.position, Range);
        return collision.gameObject.CompareTag("Player") && Timer == 0;
    }

    public override void OnStart()
    {
       IsTrue();
    }

}
