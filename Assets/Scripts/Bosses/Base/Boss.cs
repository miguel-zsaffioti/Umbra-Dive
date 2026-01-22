using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // Singleton Seguro
    public static Boss Instance { get; private set; }

    [HideInInspector] public Player player;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;

    public float maxHealth;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isDead;

    [HideInInspector] public BossHealthSystem healthSystem; 

    public BossTrigger trigger;

    public BehaviorGraphAgent behaviorAgent { get; private set; }

    protected virtual void Awake()
    {
        // Esta é a linha que estava sendo ignorada:
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        currentHealth = maxHealth;
        isDead = false;
        healthSystem = new BossHealthSystem(this);

        // ... resto do seu código de singleton
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

}