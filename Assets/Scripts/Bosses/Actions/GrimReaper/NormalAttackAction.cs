using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Normal Attack", story: "Normal Attack", category: "Action", id: "a22fd73f352bf842ae4bf8de0f2cf1b6")]
public partial class NormalAttackAction : Action
{

    private GrimReaper reaper;
    private bool hasStartedAnimation = false;

    protected override Status OnStart()
    {
        reaper = GameObject.GetComponent<GrimReaper>();

        if (reaper != null)
        {
            reaper.animator.SetTrigger("Attack");
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (reaper == null) return Status.Failure;

        var anim = reaper.animator;
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        bool isCurrentStateAttack = stateInfo.IsName("NormalAttack");

        if (!hasStartedAnimation)
        {
            if (isCurrentStateAttack) hasStartedAnimation = true;
            return Status.Running;
        }

        if (stateInfo.normalizedTime >= 0.95f || !isCurrentStateAttack)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

