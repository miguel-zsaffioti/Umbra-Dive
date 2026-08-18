using UnityEngine;

public class Doctor : Boss
{
    public GameObject areaSlashPrefab;
    public GameObject frontAttackPrefab;
    public float frontAttackSpawnDistance = 1.2f;

    private System.Action pendingAttack;

    [HideInInspector] public Vector2 frontAttackDirection;
    [HideInInspector] public Vector2 frontAttackTargetPos;

    public enum AttackTypes
    {
        Dash,
        AreaSlash,
        Front
    }


    public void SetAttackDirection(Vector2 direction)
    {
        animator.SetFloat("AttackX", direction.x);
        animator.SetFloat("AttackY", direction.y);
        animator.SetTrigger("Attack");
    }

    public void QueueAttack(System.Action attackAction)
    {
        pendingAttack = attackAction;
    }

    public void ExecuteQueuedAttack()
    {
        pendingAttack?.Invoke();
        pendingAttack = null;
    }

    private void Update()
    {
        Vector2 moveDir = moveDirection.normalized;
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;

        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("LastX", moveDir.x);
            animator.SetFloat("LastY", moveDir.y);
        }
    }

    public void AreaAttack()
    {
        if (areaSlashPrefab == null) return;
        SetAttackDirection(Vector2.down);
        Instantiate(areaSlashPrefab, transform.position, transform.rotation, transform);
        RegisterAttack("AreaSlash");
    }

    public void Dash()
    {
        RegisterAttack("Dash");
    }

    public void FrontAttack()
    {
        float angle = Mathf.Atan2(frontAttackDirection.y, frontAttackDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        float distToTarget = Vector2.Distance(transform.position, frontAttackTargetPos);
        float spawnDist = Mathf.Min(frontAttackSpawnDistance, distToTarget * 0.8f);

        Vector3 spawnPos = transform.position + (Vector3)(frontAttackDirection * spawnDist);

        Instantiate(frontAttackPrefab, spawnPos, rotation, transform);
        RegisterAttack("Front");
    }
}

