using Unity.Behavior;
using UnityEngine;

public class GrimReaper : Boss
{
    [HideInInspector] public bool isFacingRight;

    public float attackCooldown = 5f;
    public float specialCooldown = 10f;

    private float attackTimer = 0f;
    private float specialTimer = 0f;

    private BlackboardVariable<float> Range;

    public Transform slashSpawnLocation;
    public GameObject slashPrefab;

    public enum AttackTypes
    {
        Normal,
        Special
    }

    protected override void Awake()
    {
        isFacingRight = true;
        base.Awake();
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                attackTimer = 0;
                FinishCooldown(AttackTypes.Normal);
            }
        }

        if (specialTimer > 0)
        {
            specialTimer -= Time.deltaTime;

            if (specialTimer <= 0)
            {
                specialTimer = 0;
                FinishCooldown(AttackTypes.Special);
            }
        }
    }

    public void StartCooldown(AttackTypes type)
    {
        string variableName;

        if (type == AttackTypes.Normal)
        {
            attackTimer = attackCooldown;
            variableName = "AttackReady";
        }
        else // Special
        {
            specialTimer = specialCooldown;
            variableName = "SpecialReady";
        }

        if (behaviorAgent != null)
            behaviorAgent.SetVariableValue(variableName, false);
    }

    void FinishCooldown(AttackTypes type)
    {
        string variableName = type == AttackTypes.Normal ? "AttackReady" : "SpecialReady";

        if (behaviorAgent != null)
            behaviorAgent.SetVariableValue(variableName, true);
        Debug.Log($"Boss recuperou o ataque: {type}");
    }

    public void TriggerNormalAttack()
    {
        int layerMask = LayerMask.GetMask("Player");
        if (behaviorAgent.GetVariable<float>("AttackRange", out Range))
        {
            Collider2D collision = Physics2D.OverlapCircle(transform.position, Range.Value + 0.5f, layerMask);
            if (collision != null)
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    PlayerHealthSystem playerHealth = collision.gameObject.GetComponent<Player>().healthSystem;
                    if (playerHealth != null)
                    {
                        Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                        playerHealth.TakeDamage(2, knockbackDirection, 5f);
                    }
                }
            }
        }
    }

    public void TriggerSpecialAttack()
    {
        if (slashSpawnLocation == null) return;
        GameObject newSlash = Instantiate(slashPrefab, slashSpawnLocation.position, Quaternion.identity);

        Vector3 newScale = newSlash.transform.localScale;
        newScale.x = transform.localScale.x;
        newSlash.transform.localScale = newScale;

    }
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
