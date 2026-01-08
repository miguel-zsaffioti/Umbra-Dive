using System.Collections;
using UnityEngine;

public class Bat : Enemy
{
    public EnemyAttackState attackState;
    public bool canAttack = true;

    protected override void InitializeStates()
    {
        waitState = new WaitState(this, stateMachine);
        chaseState = new ChaseState(this, stateMachine);
        attackState = new EnemyAttackState(this,stateMachine);
        knockbackState = new EnemyKnockbackState(this, stateMachine);

        stateMachine.Start(waitState);
    }

    public void AnimationTriggerEvent(string eventName)
    {
        stateMachine.OnAnimationTrigger(eventName);
    }

    public IEnumerator waitToAttack() { 
        canAttack = false;
        yield return new WaitForSeconds(data.attackCooldown);
        canAttack = true;
    }
}
