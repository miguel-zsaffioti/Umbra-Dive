using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Special Attack", story: "Special Attack", category: "Action", id: "cb7423d5440f479d72c6dd3f38eb9a29")]
public partial class SpecialAttackAction : Action
{
    private bool hasStartedAnimation = false;

    private GrimReaper reaper;

    protected override Status OnStart()
    {
        hasStartedAnimation = false;

        reaper = GameObject.GetComponent<GrimReaper>();

        if (reaper != null)
        {
            reaper.animator.SetTrigger("Special");
        }
        else
        {
            Debug.LogError("Erro: O script GrimReaper não foi encontrado no objeto!");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (reaper == null) return Status.Failure;

        var anim = reaper.animator;
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool isCurrentStateSpecial = stateInfo.IsName("SpecialAttack");

        if (!hasStartedAnimation)
        {
            if (isCurrentStateSpecial)
            {
                hasStartedAnimation = true;
            }
            return Status.Running;
        }

        if (stateInfo.normalizedTime >= 0.95f || !isCurrentStateSpecial)
        {
            return Status.Success;
        }

        return Status.Running;
    }

}