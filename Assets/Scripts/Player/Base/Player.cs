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
    private PlayerStateMachine stateMachine;
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
    #endregion

    #region State Variables
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Roll")]
    public AnimationClip rollAnimation;
    public float rollForce = 5f;

    [Header("Attack")]
    public AnimationClip attackAnimation;
    public float attackMoveSpeed = 2f;

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

    [HideInInspector] public List<Enemy> enemiesAlreadyHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        enemiesAlreadyHit = new List<Enemy>();

        inputSystem = new PlayerInputSystem();
        stateMachine = new PlayerStateMachine();
        healthSystem = new PlayerHealthSystem(this);
        audioSystem = new PlayerAudioSystem(this);
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
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    public void TriggerAttackDamage()
    {
        attackPoint.localPosition = this.facingDirection * attackPointOffset;

        // 1. Criamos uma flag para controlar o Hit Stop
        bool hasHitEnemy = false;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();

            if (enemy != null && !enemiesAlreadyHit.Contains(enemy))
            {
                hasHitEnemy = true;
                

                Debug.Log("Acertamos: " + enemy.name);
                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                enemy.healthSystem.TakeDamage(attackDamage, knockbackDirection, enemy.data.knockbackForce);
                enemiesAlreadyHit.Add(enemy);
            }
        }
        audioSystem.PlayAttackSound(hasHitEnemy);
        Debug.Log("entrando em triugger attack");
        if (hasHitEnemy)
        {
            StartCoroutine(HitStopEffect(0.08f));
        }
    }

    // Usamos IEnumerator para criar uma Coroutine
    private IEnumerator HitStopEffect(float duration)
    {
        Time.timeScale = 0.1f;

  
        yield return new WaitForSecondsRealtime(duration);

        // Retorna o tempo ao normal
        Time.timeScale = 1f;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.linearVelocity = Vector2.zero;

        rb.AddForce(direction * force, ForceMode2D.Impulse);

        stateMachine.ChangeState(knockbackState);
    }
}
