using System.Xml;
using UnityEngine;

// Torna a classe abstrata. Você não poderá adicionar o componente "Enemy" diretamente
// em um GameObject, apenas classes que herdam dele (como "Slime").
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

    [HideInInspector] public bool isFacingRight = true;

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

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        this.InitializeStates();
        currentHealth = maxHealth;
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
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}