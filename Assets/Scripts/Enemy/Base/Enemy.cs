using UnityEngine;

public class Enemy : MonoBehaviour
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

    [HideInInspector] public EnemyStateMachine stateMachine;
    [HideInInspector] public WaitState waitState;
    [HideInInspector] public ChaseState chaseState;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stateMachine = new EnemyStateMachine();
        detectionSystem = new DetectionSystem(this);
        healthSystem = new EnemyHealthSystem(this);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        waitState = new WaitState(this, stateMachine);
        chaseState = new ChaseState(this, stateMachine);

        stateMachine.Start(waitState);
        currentHealth = maxHealth;
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
    }
}
