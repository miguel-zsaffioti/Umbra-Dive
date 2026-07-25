using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class Boss : MonoBehaviour, IDamagable
{
    public static Boss Instance { get; private set; }

    [HideInInspector] public Player player;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;

    public int maxHealth = 100;
    [HideInInspector] public BossHealthSystem healthSystem;
    [HideInInspector] public BossTrigger trigger;

    public int CurrentHealth => healthSystem.CurrentHealth;
    public bool IsDead => healthSystem.IsDead;

    public BehaviorGraphAgent behaviorAgent { get; private set; }

    public event System.Action<int, int> OnHealthChanged;

    public string bossName; 

    public void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        healthSystem = new BossHealthSystem(this, maxHealth);

        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (Player.Instance != null)
        {
            player = Player.Instance;
        }
        else
        {
            player = FindFirstObjectByType<Player>();
        }

        NotifyHealthChanged();
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Die()
    {
        StartCoroutine(WaitToDie());
        if (trigger == null) return;
        trigger.Door.SetActive(false);
    }

    private IEnumerator WaitToDie()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public Player GetPlayerInRange(float range)
    {
        int layerMask = LayerMask.GetMask("Player");
        Collider2D collision = Physics2D.OverlapCircle(transform.position, range, layerMask);
        if (collision == null) return null;
        return collision.gameObject.GetComponent<Player>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        healthSystem.TakeDamage(damage);
        //Debug.Log("Tomou dano");
    }
}