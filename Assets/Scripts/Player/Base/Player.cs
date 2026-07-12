using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;
    #endregion

    #region Systems
    [HideInInspector] public PlayerInputSystem inputSystem;
    [HideInInspector] public PlayerHealthSystem healthSystem;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public PlayerAudioSystem audioSystem;
    [HideInInspector] public PlayerStateMachine stateMachine;
    #endregion

    #region Concrete States
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkState walkState;
    [HideInInspector] public RollState rollState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public BlockState blockState;
    [HideInInspector] public KnockbackState knockbackState;
    #endregion

    #region Health
    [Header("Health")]
    public int maxHealth = 100;
    [HideInInspector] public int currentHealth;
    public float invincibilityDuration = 1.0f;
    [HideInInspector] public bool isInvincible = false;
    [HideInInspector] public bool isBlocking = false;
    public float blockBufferDuration = 0.2f;
    #endregion

    #region State Variables
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Roll")]
    public AnimationClip rollAnimation;
    public float rollForce = 5f;
    public float rollCooldownDuration = 1f;

    [HideInInspector] public Cooldown rollCooldown;

    [Header("Attack")]
    public AnimationClip attackAnimation;
    public float attackMoveSpeed = 2f;
    public float windUpDuration = 0.15f;
    public float squashAmount = 0.9f;
    public float stretchAmount = 1.1f;
    public float scaleSmoothness = 20f;

    [Header("Knockback")]
    public float knockbackDuration = 0.2f;
    #endregion


    [HideInInspector] public Vector2 facingDirection;

    [Header("Configurações de Ataque")]
    public Transform attackPoint;
    public float attackPointOffset = 1f;
    public float attackRange = 0.8f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;

    [Header("Data")]
    public PlayerAudioData audioData;

    private Coroutine layerWeightCoroutine;
    private Coroutine hitStopCoroutine;

    [HideInInspector] public List<GameObject> objectsAlreadyHit = new List<GameObject>();

    public static Player Instance;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        inputSystem = new PlayerInputSystem();
        stateMachine = new PlayerStateMachine();
        healthSystem = new PlayerHealthSystem(this);
        audioSystem = new PlayerAudioSystem(this);
        rollCooldown = new Cooldown(rollCooldownDuration);
    }

    #region Enable/Disable
    private void OnEnable()
    {
        inputSystem.Enable();
    }

    private void OnDisable()
    {
        inputSystem.Disable();
    }
    #endregion

    void Start()
    {
        idleState = new IdleState(this, stateMachine);
        walkState = new WalkState(this, stateMachine);
        rollState = new RollState(this, stateMachine);
        attackState = new AttackState(this, stateMachine);
        blockState = new BlockState(this, stateMachine);
        knockbackState = new KnockbackState(this, stateMachine);

        stateMachine.Start(idleState);
        currentHealth = maxHealth;
    }

    void Update()
    {
        inputSystem.Tick();
        stateMachine.FrameUpdate();
        rollCooldown.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    public void TriggerAttackDamage()
    {
        attackPoint.localPosition = this.facingDirection * attackPointOffset;

        bool hasHitSomething = false;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            Boss boss = hitCollider.GetComponent<Boss>();

            if (enemy != null && !objectsAlreadyHit.Contains(enemy.gameObject))
            {
                hasHitSomething = true;

                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                enemy.healthSystem.TakeDamage(attackDamage, knockbackDirection, 10f);

                objectsAlreadyHit.Add(enemy.gameObject);
            }
            else if (boss != null && !objectsAlreadyHit.Contains(boss.gameObject))
            {
                hasHitSomething = true;

                Vector2 knockbackDirection = (boss.transform.position - transform.position).normalized;

                boss.healthSystem.TakeDamage(attackDamage);

                objectsAlreadyHit.Add(boss.gameObject);
            }
          }
        if (hasHitSomething)
        {
            StartHitStop(0.08f);
        }
    }
    private void StartHitStop(float duration)
    {
        if (hitStopCoroutine != null)
            StopCoroutine(hitStopCoroutine);

        hitStopCoroutine = StartCoroutine(HitStopEffect(duration));
    }

    private IEnumerator HitStopEffect(float duration)
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        hitStopCoroutine = null;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.linearVelocity = Vector2.zero;

        rb.AddForce(direction * force, ForceMode2D.Impulse);

        stateMachine.ChangeState(knockbackState);
    }

    public IEnumerator StopBlocking()
    {
        yield return new WaitForSecondsRealtime(blockBufferDuration);
        isBlocking = false;
    }

    public void EnableCombatLayer()
    {
        if (layerWeightCoroutine != null) StopCoroutine(layerWeightCoroutine);
        animator.SetLayerWeight(1, 1f); // Liga instantaneamente para o ataque sair na hora
    }

}
