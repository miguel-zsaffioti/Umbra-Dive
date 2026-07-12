using UnityEngine;

public class AttackState : PlayerState
{
    private float attackTimer;
    private bool animationPlayed;
    private Vector2 attackDirection;
    private Vector3 normalScale;
    private Vector3 targetScale;

    private bool bufferedAttack;

    public AttackState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        player.animator.Play("Empty", 1, 0f);
        player.EnableCombatLayer();
        animationPlayed = false;
        bufferedAttack = false;

        if (inputSystem.MoveInput.magnitude > 0.1f)
        {
            attackDirection = inputSystem.MoveInput.normalized;
        }
        else
        {
            attackDirection = new Vector2(animator.GetFloat("LastX"), animator.GetFloat("LastY"));
        }

        animator.SetFloat("LastX", attackDirection.x);
        animator.SetFloat("LastY", attackDirection.y);


        attackTimer = player.attackAnimation.length;

        normalScale = player.transform.localScale;
        targetScale = normalScale;

        player.facingDirection = attackDirection;
        player.objectsAlreadyHit.Clear();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (inputSystem.AttackInput && attackTimer <= player.attackAnimation.length * 0.3f)
        {
            bufferedAttack = true;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            if (bufferedAttack) 
                stateMachine.ChangeState(player.attackState);
            else if (inputSystem.MoveInput == Vector2.zero)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.walkState);
        }

        player.transform.localScale = Vector3.Lerp(player.transform.localScale, targetScale, Time.deltaTime * player.scaleSmoothness);
    }

    public override void PhysicsUpdate()
    {
        float recoveryDuration = player.attackAnimation.length * 0.2f;
        float timeAfterWindup = player.attackAnimation.length - player.windUpDuration;

        if (attackTimer > timeAfterWindup)
        {
            CalculateTargetScale(true); // Squash
            rb.linearVelocity = -attackDirection * (player.attackMoveSpeed * 0.3f);
        }
        else
        {
            if (!animationPlayed)
            {
                animator.SetTrigger("Attack");
                animationPlayed = true;
            }

            if (attackTimer > recoveryDuration)
            {
                CalculateTargetScale(false); 
            }
            else
            {
                targetScale = normalScale; 
            }

            float progress = attackTimer / timeAfterWindup;
            rb.linearVelocity = attackDirection * player.attackMoveSpeed * progress;
        }
    }

    public override void Exit()
    {
        player.animator.SetLayerWeight(1, 0f);

        if (player.inputSystem.MoveInput != Vector2.zero)
        {
            player.animator.Play("Walk", 0, 0f);
        }
        else
        {
            player.animator.Play("Idle", 0, 0f);
        }

        player.animator.SetFloat("LastX", player.facingDirection.x);
        player.animator.SetFloat("LastY", player.facingDirection.y);

        player.animator.Update(0f);

        float direction = (player.transform.localScale.x > 0) ? 1f : -1f;
        player.transform.localScale = new Vector3(Mathf.Abs(normalScale.x) * direction, normalScale.y, normalScale.z);
    }

    private void CalculateTargetScale(bool isWindup)
    {
        float xMult = 1;
        float yMult = 1;

        if (Mathf.Abs(attackDirection.x) > Mathf.Abs(attackDirection.y))
        {
            if (isWindup)
            {
                xMult = player.squashAmount;
                yMult = 2f - player.squashAmount;
            }
            else
            {
                xMult = player.stretchAmount;
                yMult = 2f - player.stretchAmount;
            }
        }
        else
        {
            if (isWindup)
            {
                xMult = 2f - player.squashAmount;
                yMult = player.squashAmount;
            }
            else
            {
                xMult = 2f - player.stretchAmount;
                yMult = player.stretchAmount;
            }
        }

        targetScale = new Vector3(normalScale.x * xMult, normalScale.y * yMult, normalScale.z);
    }
}