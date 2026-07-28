using Unity.Behavior;
using UnityEngine;

public class GrimReaper : Boss
{
    [HideInInspector] public bool isFacingRight;

    public float attackCooldownDuration = 5f;
    public float specialCooldownDuration = 10f;

    private Cooldown attackCooldown;
    private Cooldown specialCooldown;

    public float attackRange = 1.5f;

    public Transform slashSpawnLocation;
    public GameObject slashPrefab;

    [Header("Flip")]
    public float flipThreshold = 0.3f;
    public float flipCooldown = 0.2f;
    private float lastFlipTime = -999f;

    public enum AttackTypes
    {
        Normal,
        Special
    }

    protected override void Awake()
    {
        isFacingRight = true;
        attackCooldown = new Cooldown(attackCooldownDuration);
        specialCooldown = new Cooldown(specialCooldownDuration);
        base.Awake();
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    public void TryFlipTowards(float diffX)
    {
        if (Time.time - lastFlipTime < flipCooldown) return;
        if (Mathf.Abs(diffX) < flipThreshold) return;

        if (diffX > 0 && !isFacingRight)
        {
            Flip();
            lastFlipTime = Time.time;
        }
        else if (diffX < 0 && isFacingRight)
        {
            Flip();
            lastFlipTime = Time.time;
        }
    }

    private void Update()
    {
        attackCooldown.Tick(Time.deltaTime);
        specialCooldown.Tick(Time.deltaTime);

        behaviorAgent.SetVariableValue("AttackReady", attackCooldown.IsReady);
        behaviorAgent.SetVariableValue("SpecialReady", specialCooldown.IsReady);
    }

    public void TriggerNormalAttack()
    {
        Player player = GetPlayerInRange(attackRange + 0.5f);

        if (player != null && player.healthSystem != null)
        {
            Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
            player.healthSystem.TakeDamage(2, knockbackDirection, 5f);
        }

        attackCooldown.Reset();
    }

    public void TriggerSpecialAttack()
    {
        if (slashSpawnLocation == null) return;
        GameObject newSlash = Instantiate(slashPrefab, slashSpawnLocation.position, Quaternion.identity);

        Vector3 newScale = newSlash.transform.localScale;
        newScale.x = transform.localScale.x;
        newSlash.transform.localScale = newScale;
        specialCooldown.Reset();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}