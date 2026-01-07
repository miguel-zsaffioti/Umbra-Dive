using UnityEngine;

public class AttackState : PlayerState
{
    private float attackTimer;
    private bool animationPlayed;
    private float windUpDuration = 0.15f; // Tempo de preparação
    private Vector2 attackDirection;
    private Vector3 normalScale;
    private Vector3 targetScale; // Para onde a escala deve ir

    // Configurações de "Juiciness"
    private float squashAmount = 0.8f; // Quanto encolhe na preparação
    private float stretchAmount = 1.1f; // Quanto estica no ataque
    private float scaleSmoothness = 20f; // Velocidade da transição (Lerp)

    public AttackState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        player.animator.Play("Empty", 1, 0f);
        player.EnableCombatLayer();
        animationPlayed = false;

        // 1. Define a direção baseada no Input ou na última posição
        if (inputSystem.MoveInput.magnitude > 0.1f)
        {
            attackDirection = inputSystem.MoveInput.normalized;
        }
        else
        {
            attackDirection = new Vector2(animator.GetFloat("LastX"), animator.GetFloat("LastY"));
        }

        // 2. CORREÇÃO: Avisar o Animator da nova direção IMEDIATAMENTE
        // Isso garante que quando voltar pro Idle, ele saiba para onde estamos olhando
        animator.SetFloat("LastX", attackDirection.x);
        animator.SetFloat("LastY", attackDirection.y);

        // Reinicia timer e flags
        attackTimer = player.attackAnimation.length;

        // Configuração de Escala
        normalScale = player.transform.localScale;
        targetScale = normalScale;

        player.facingDirection = attackDirection;
        player.enemiesAlreadyHit.Clear();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        // Atualiza timer
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            if (inputSystem.MoveInput == Vector2.zero)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.walkState);
        }

        // APLICAÇÃO VISUAL (LERP)
        // Isso acontece no FrameUpdate para ser suave visualmente (independente da física)
        player.transform.localScale = Vector3.Lerp(player.transform.localScale, targetScale, Time.deltaTime * scaleSmoothness);
    }

    public override void PhysicsUpdate()
    {
        // Definir quanto tempo do final da animação serve para voltar ao normal
        // Ex: últimos 20% da animação são para "esfriar" o sprite
        float recoveryDuration = player.attackAnimation.length * 0.2f;
        float timeAfterWindup = player.attackAnimation.length - windUpDuration;

        // FASE 1: WINDUP (Preparação)
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

            // Dica extra: Use Mathf.Sqrt ou curva para o movimento não ser linear demais
            rb.linearVelocity = attackDirection * player.attackMoveSpeed * progress;
        }
    }

    public override void Exit()
    {
        // 1. MATA O PESO DA LAYER DE COMBATE IMEDIATAMENTE
        // Não use o Smoothly aqui. Se a layer de ataque tiver peso 0.1 que seja,
        // e estiver num estado vazio, ela vai sobrepor o Idle da base com o sprite padrão.
        player.animator.SetLayerWeight(1, 0f);

        // 2. Toca a animação correta na base
        if (player.inputSystem.MoveInput != Vector2.zero)
        {
            player.animator.Play("Walk", 0, 0f);
        }
        else
        {
            player.animator.Play("Idle", 0, 0f);
        }

        // 3. Atualiza os parametros
        player.animator.SetFloat("LastX", player.facingDirection.x);
        player.animator.SetFloat("LastY", player.facingDirection.y);

        // 4. Força o Animator a processar essa mudança NESTE frame
        // Isso impede que ele espere até o próximo Update para trocar o sprite
        player.animator.Update(0f);

        // 5. Restaura a escala (Sua lógica original mantida)
        float direction = (player.transform.localScale.x > 0) ? 1f : -1f;
        player.transform.localScale = new Vector3(Mathf.Abs(normalScale.x) * direction, normalScale.y, normalScale.z);
    }

    // isWindup = true (espremer/preparar), false (esticar/atacar)
    private void CalculateTargetScale(bool isWindup)
    {
        float xMult = 1;
        float yMult = 1;

        // Se o ataque é Horizontal
        if (Mathf.Abs(attackDirection.x) > Mathf.Abs(attackDirection.y))
        {
            if (isWindup)
            {
                // Preparação: Encolhe na largura (X), Cresce na altura (Y) -> Efeito de recuar
                xMult = squashAmount;
                yMult = 2f - squashAmount; // Mantém volume aproximado (ex: 0.8 e 1.2)
            }
            else
            {
                // Ataque: Estica na direção do ataque (X), Encolhe na altura (Y) -> Efeito de velocidade
                xMult = stretchAmount;
                yMult = 2f - stretchAmount;
            }
        }
        // Se o ataque é Vertical
        else
        {
            if (isWindup)
            {
                // Preparação: Encolhe na altura (Y), Cresce na largura (X) -> Efeito de agachar
                xMult = 2f - squashAmount;
                yMult = squashAmount;
            }
            else
            {
                // Ataque: Estica na altura (Y), Encolhe na largura (X)
                xMult = 2f - stretchAmount;
                yMult = stretchAmount;
            }
        }

        targetScale = new Vector3(normalScale.x * xMult, normalScale.y * yMult, normalScale.z);
    }
}