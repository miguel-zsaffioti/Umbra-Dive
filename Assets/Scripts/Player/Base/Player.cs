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


    public float moveSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        inputSystem = new PlayerInputSystem();
        stateMachine = new PlayerStateMachine();

        idleState = new IdleState(this, stateMachine);
        walkState = new WalkState(this, stateMachine);
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
