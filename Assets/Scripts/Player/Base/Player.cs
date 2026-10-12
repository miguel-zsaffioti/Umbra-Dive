using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Unity.Cinemachine;

public class Player : MonoBehaviour, IDamagable
{
    #region Components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;
    private AudioSource footstepSource;
    #endregion

    #region Systems
    [HideInInspector] public PlayerInputSystem inputSystem;
    [HideInInspector] public PlayerHealthSystem healthSystem;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public PlayerStateMachine stateMachine;
    #endregion

    #region Concrete States
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkState walkState;
    [HideInInspector] public RollState rollState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public BlockState blockState;
    [HideInInspector] public KnockbackState knockbackState;
    #endregion

    #region Health
    [Header("Health")]
    public int maxHealth = 100;
    [HideInInspector] public int currentHealth;
    public float invincibilityDuration = 1.0f;
    [HideInInspector] public bool isInvincible = false;
    [HideInInspector] public bool isBlocking = false;
    public float blockBufferDuration = 0.2f;
    public int cureAmount = 1;
    public float cureTick = 0.5f;

    public event System.Action<int, int> OnHealthChanged; // (current, max)

    public void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    #endregion

    #region Potions
    [Header("Potions")]
    public int maxPotions = 3;
    [HideInInspector] public int currentPotions;
    public int potionHealAmount = 30;

    public event System.Action<int, int> OnPotionsChanged; // (current, max)

    public void NotifyPotionsChanged()
    {
        OnPotionsChanged?.Invoke(currentPotions, maxPotions);
    }

    public void TryUsePotion()
    {
        if (currentPotions <= 0) return;
        if (currentHealth >= maxHealth) return;
        if (stateMachine.currState == knockbackState) return;

        currentPotions--;
        NotifyPotionsChanged();

        healthSystem.RestoreHealthAmount(potionHealAmount);

        PlaySound(audioData.potionUse);
    }

    public void RestorePotions()
    {
        currentPotions = maxPotions;
        NotifyPotionsChanged();
    }
    #endregion

    #region State Variables
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Roll")]
    public AnimationClip rollAnimation;
    public float rollForce = 5f;
    public float rollCooldownDuration = 1f;

    [HideInInspector] public Cooldown rollCooldown;

    [Header("Attack")]
    public AnimationClip attackAnimation;
    public float attackMoveSpeed = 2f;
    public float windUpDuration = 0.15f;
    public float squashAmount = 0.9f;
    public float stretchAmount = 1.1f;
    public float scaleSmoothness = 20f;

    [Header("Knockback")]
    public float knockbackDuration = 0.2f;
    #endregion


    [HideInInspector] public Vector2 facingDirection;

    [Header("Configurações de Ataque")]
    public Transform attackPoint;
    public float attackPointOffset = 1f;
    public float attackRange = 0.8f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;

    [Header("Data")]
    public PlayerAudioData audioData;

    private Coroutine layerWeightCoroutine;
    private Coroutine hitStopCoroutine;

    [HideInInspector] public List<GameObject> objectsAlreadyHit = new List<GameObject>();

    public static Player Instance;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.playOnAwake = false;
        if (audioSource != null)
        {
            footstepSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            footstepSource.spatialBlend = audioSource.spatialBlend;
            footstepSource.volume = audioSource.volume;
        }

        inputSystem = new PlayerInputSystem();
        stateMachine = new PlayerStateMachine();
        healthSystem = new PlayerHealthSystem(this);
        rollCooldown = new Cooldown(rollCooldownDuration);
    }

    #region Audio
    public void PlaySound(AudioResource resource)
    {
        if (resource == null) return;

        audioSource.resource = resource;
        audioSource.Play();
    }

    public void PlayFootstep()
    {
        if (stateMachine.currState != walkState) return;

        if (audioData == null || audioData.footsteps == null) return;

        footstepSource.resource = audioData.footsteps;
        footstepSource.Play();
    }
    #endregion

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
        idleState = new IdleState(this, stateMachine);
        walkState = new WalkState(this, stateMachine);
        rollState = new RollState(this, stateMachine);
        attackState = new AttackState(this, stateMachine);
        blockState = new BlockState(this, stateMachine);
        knockbackState = new KnockbackState(this, stateMachine);

        stateMachine.Start(idleState);
        currentHealth = maxHealth;
        NotifyHealthChanged();

        currentPotions = maxPotions;
        NotifyPotionsChanged();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        inputSystem.Tick();
        stateMachine.FrameUpdate();
        rollCooldown.Tick(Time.deltaTime);

        if (inputSystem.HealInput)
        {
            TryUsePotion();
        }
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    public void TriggerAttackDamage()
    {
        attackPoint.localPosition = this.facingDirection * attackPointOffset;

        bool hasHitSomething = false;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamagable damageable = hitCollider.GetComponent<IDamagable>();

            if (damageable != null && !objectsAlreadyHit.Contains(hitCollider.gameObject))
            {
                hasHitSomething = true;

                Vector2 knockbackDirection = (hitCollider.transform.position - transform.position).normalized;
                damageable.TakeDamage(attackDamage, knockbackDirection, 10f);

                objectsAlreadyHit.Add(hitCollider.gameObject);
            }
        }

        if (hasHitSomething)
        {
            PlaySound(audioData.swordImpact);
            StartHitStop(0.08f);
        }
        else
        {
            PlaySound(audioData.swordSlash);
        }
    }

    private void StartHitStop(float duration)
    {
        if (hitStopCoroutine != null)
            StopCoroutine(hitStopCoroutine);

        hitStopCoroutine = StartCoroutine(HitStopEffect(duration));
    }

    private IEnumerator HitStopEffect(float duration)
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(duration);

        if (Time.timeScale > 0f)
        {
            Time.timeScale = 1f;
        }

        hitStopCoroutine = null;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.linearVelocity = Vector2.zero;

        rb.AddForce(direction * force, ForceMode2D.Impulse);

        stateMachine.ChangeState(knockbackState);
    }

    public IEnumerator StopBlocking()
    {
        yield return new WaitForSecondsRealtime(blockBufferDuration);
        isBlocking = false;
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        healthSystem.TakeDamage(damage, knockbackDirection, knockbackForce);
    }

    public void EnableCombatLayer()
    {
        if (layerWeightCoroutine != null) StopCoroutine(layerWeightCoroutine);
        animator.SetLayerWeight(1, 1f);
    }

    public void RestoreHealth()
    {
        StartCoroutine(healthSystem.RestoreHealth(cureAmount, cureTick));
    }

    public void Teleport(Vector2 position)
    {
        Vector3 delta = (Vector3)position - transform.position;

        transform.position = position;
        if (rb != null)
        {
            rb.position = position;
            rb.linearVelocity = Vector2.zero;
        }

        foreach (CinemachineCamera cam in FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None))
        {
            cam.OnTargetObjectWarped(transform, delta);
        }
    }

    public void Respawn(Vector2 position)
    {
        gameObject.SetActive(true);
        Teleport(position);

        isInvincible = false;
        isBlocking = false;
        spriteRenderer.enabled = true;

        objectsAlreadyHit.Clear();
        stateMachine.ChangeState(idleState);
    }

}