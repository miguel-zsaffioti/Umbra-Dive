using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    #endregion

    #region Systems
    [HideInInspector] public PlayerInputSystem inputSystem;
    [HideInInspector] public HealthSystem healthSystem;
    [HideInInspector] public SpriteRenderer spriteRenderer;
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        inputSystem = new PlayerInputSystem();
        stateMachine = new PlayerStateMachine();
        healthSystem = new HealthSystem(this);  
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

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.linearVelocity = Vector2.zero;

        rb.AddForce(direction * force, ForceMode2D.Impulse);

        stateMachine.ChangeState(knockbackState);
    }
}
