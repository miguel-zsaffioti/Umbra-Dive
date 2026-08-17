using UnityEngine;

public class Doctor : Boss
{
    public GameObject areaSlashPrefab;
    public GameObject frontAttackPrefab;
    public float frontAttackSpawnDistance = 1.2f;

    [HideInInspector] public Vector2 frontAttackDirection;
    public enum AttackTypes
    {
        Dash,
        AreaSlash,
        Front
    }


    private void SetAttackDirection(Vector2 direction)
    {
        animator.SetFloat("AttackX", direction.x);
        animator.SetFloat("AttackY", direction.y);
        animator.SetTrigger("Attack");
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
        SetAttackDirection(frontAttackDirection);

        float angle = Mathf.Atan2(frontAttackDirection.y, frontAttackDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3 spawnPos = transform.position + (Vector3)(frontAttackDirection * frontAttackSpawnDistance);

        Instantiate(frontAttackPrefab, spawnPos, rotation, transform);
        RegisterAttack("Front");
    }
}

