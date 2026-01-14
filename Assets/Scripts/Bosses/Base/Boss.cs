using UnityEngine;

public class Boss : MonoBehaviour
{
    [HideInInspector] public Player player;

    [HideInInspector] public static Boss Instance;
    [HideInInspector] public bool isFacingRight;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;


    private void Awake()
    {
       player = Player.Instance;
       isFacingRight = true;
       rb = GetComponent<Rigidbody2D>();
       animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void TriggerNormalAttack() { 
        
    }

    public void TriggerSpecialAttack()
    {

    }
}

