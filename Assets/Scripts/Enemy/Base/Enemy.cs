using System.Xml;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IDamagable
{
    [Header("Data")]
    public EnemyData data;

    [Header("Components")]
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public NavMeshAgent agent;

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

    [HideInInspector] public Vector2 spawnPosition;

    [HideInInspector] public EnemyStateMachine stateMachine;

    [HideInInspector] public bool startAggroed = false;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stateMachine = new EnemyStateMachine();
        detectionSystem = new DetectionSystem(this);
        healthSystem = new EnemyHealthSystem(this);
        knockbackState = new EnemyKnockbackState(this, stateMachine);

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }


    }

    private void Start()
    {
        this.InitializeStates();
        currentHealth = maxHealth;
        spawnPosition = transform.position;
        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }

        if (agent != null && data != null)
        {

            agent.speed = data.moveSpeed;
            agent.acceleration = 999f;
            agent.stoppingDistance = 0f;
            agent.autoBraking = false;
        }
    }

    public virtual bool TryAttack()
    {
        return false;
    }

    public void FaceDirection(Vector2 direction)
    {
        if (direction.x > 0 && !isFacingRight) Flip();
        else if (direction.x < 0 && isFacingRight) Flip();
    }

    

    protected virtual void InitializeStates()
    {
        waitState = new WaitState(this, stateMachine);
        chaseState = new ChaseState(this, stateMachine);

        stateMachine.Start(startAggroed ? chaseState : waitState);
    }

    public void ForceChase()
    {
        startAggroed = true;
        detectionSystem?.ForceDetect();
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

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        healthSystem.TakeDamage(damage, knockbackDirection, knockbackForce);
    }

}