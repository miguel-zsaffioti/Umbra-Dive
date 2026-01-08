using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter()
    {
        enemy.animator.SetBool("isChasing", true);
    }

    public override void Exit()
    {
        enemy.animator.SetBool("isChasing", false);
    }

    public override void FrameUpdate()
    {
        if (!enemy.detectionSystem.IsPlayerDetected())
        {
            stateMachine.ChangeState(enemy.waitState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        if (enemy.playerTransform == null) return;

        Vector2 dir = (enemy.playerTransform.position - enemy.transform.position);
        float distanceToPlayer = dir.magnitude;

        // --- CORREÇÃO DO SAMBA (Flip) ---
        if (Mathf.Abs(dir.x) > 0.2f)
        {
            if (dir.x > 0 && !enemy.isFacingRight) enemy.Flip();
            else if (dir.x < 0 && enemy.isFacingRight) enemy.Flip();
        }

        // Lógica de Ataque
        if (distanceToPlayer <= enemy.data.attackDistance && ((Bat)enemy).canAttack)
        {
            // Importante: Zere a velocidade/inércia ao atacar
            enemy.rb.linearVelocity = Vector2.zero;
            stateMachine.ChangeState(((Bat)enemy).attackState);
            return;
        }
        // Lógica de Movimento
        else if (distanceToPlayer > enemy.data.stopDistance)
        {
            // MovePosition é mais estável que AddForce para RPGs
            // Calculamos a nova posição baseada na velocidade * tempo fixo
            Vector2 targetPosition = enemy.rb.position + (dir.normalized * enemy.data.moveSpeed * Time.fixedDeltaTime);

            enemy.rb.MovePosition(targetPosition);
        }
        // Parado
        else
        {
            // Se já chegou perto, garanta que ele não deslize
            enemy.rb.linearVelocity = Vector2.zero;
        }
    }
}
