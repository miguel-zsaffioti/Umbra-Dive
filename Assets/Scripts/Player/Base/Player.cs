using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    private Rigidbody2D rb;
    private Animator animator;
    #endregion

    #region Systems
    private PlayerInputSystem inputSystem;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        inputSystem = new PlayerInputSystem();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputSystem.Tick();
    }

    private void OnEnable()
    {
        inputSystem.Enable();
    }

    private void OnDisable()
    {
        inputSystem.Disable();
    }
}
