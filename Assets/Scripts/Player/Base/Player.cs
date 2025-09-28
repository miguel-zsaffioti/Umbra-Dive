using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    #endregion

    #region Systems
    [HideInInspector] public PlayerInputSystem inputSystem;
    private PlayerStateMachine stateMachine;
    #endregion

    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkState walkState;
    [HideInInspector] public RollState rollState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public BlockState blockState;


    public float moveSpeed = 5f;

    public AnimationClip rollAnimation;
    public float rollForce = 5f;

    public AnimationClip attackAnimation;
    public float attackMoveSpeed = 2f;

    public float blockMoveSpeed = 3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        inputSystem = new PlayerInputSystem();
        stateMachine = new PlayerStateMachine();

        idleState = new IdleState(this, stateMachine);
        walkState = new WalkState(this, stateMachine);
        rollState = new RollState(this, stateMachine);
        attackState = new AttackState(this, stateMachine);
        blockState = new BlockState(this, stateMachine);
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
        stateMachine.Start(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        inputSystem.Tick();
        stateMachine.FrameUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

}
