using System.Xml;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Data")]
    public EnemyData data;

    [Header("Components")]
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTransform;

    [Header("Detection")]
    [HideInInspector] public DetectionSystem detectionSystem;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Health")]
    public int maxHealth = 2;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public EnemyHealthSystem healthSystem;

    public WaitState waitState;
    public ChaseState chaseState;
    public EnemyKnockbackState knockbackState;

    [HideInInspector] public bool isFacingRight = true;

    [HideInInspector] public bool isDead = false;

    // A máquina de estados continua aqui, mas os estados específicos saem.
    [HideInInspector] public EnemyStateMachine stateMachine;

    // Awake permanece igual, pois inicializa componentes comuns a todos.
    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stateMachine = new EnemyStateMachine();
        detectionSystem = new DetectionSystem(this);
        healthSystem = new EnemyHealthSystem(this);
        knockbackState = new EnemyKnockbackState(this, stateMachine);

        
    }

    private void Start()
    {
        this.InitializeStates();
        currentHealth = maxHealth;
        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }
    }

    protected virtual void InitializeStates()
    {
        waitState = new WaitState(this, stateMachine);
        chaseState = new ChaseState(this, stateMachine);

        stateMachine.Start(waitState);
    }

    public virtual void Update()
    {
        stateMachine.FrameUpdate();
        detectionSystem.UpdateDetection();
    }

    public virtual void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.playerDetectionRadius);
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        stateMachine.ChangeState(knockbackState);
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}