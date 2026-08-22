using System.Collections.Generic;
using UnityEngine;

public class Doctor : Boss
{
    public GameObject areaSlashPrefab;
    public GameObject frontAttackPrefab;
    public GameObject areaAttackPrefab;
    public GameObject portalPrefab;
    public float frontAttackSpawnDistance = 1.2f;

    [HideInInspector] public Vector2 arenaCenter;

    private System.Action pendingAttack;

    [HideInInspector] public Vector2 frontAttackDirection;
    [HideInInspector] public Vector2 frontAttackTargetPos;

    public enum AttackTypes
    {
        Dash,
        AreaSlash,
        Front,
        Area,
        Summon
    }

    protected override void Start()
    {
        base.Start();
        arenaCenter = arenaBounds.bounds.center;
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

    public List<Vector2> GetRandomArenaPoints(int count, float minDistance = 2f, float minDistanceFromBoss = 2f, int maxAttempts = 100)
    {
        List<Vector2> points = new List<Vector2>();
        Bounds bounds = arenaBounds.bounds;
        Vector2 bossPos = transform.position;
        int attempts = 0;

        while (points.Count < count && attempts < maxAttempts)
        {
            attempts++;

            Vector2 point = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            if (!arenaBounds.OverlapPoint(point)) continue;
            if (Vector2.Distance(point, bossPos) < minDistanceFromBoss) continue;

            bool tooClose = points.Exists(p => Vector2.Distance(p, point) < minDistance);
            if (!tooClose) points.Add(point);
        }

        return points;
    }

    public void MultipleAreaAttack()
    {
        int qtd = Random.Range(2, 4);
        List<Vector2> poisonSpots = GetRandomArenaPoints(qtd, minDistance: 2f);
        foreach (Vector2 spot in poisonSpots)
        {
            Instantiate(areaAttackPrefab, spot, UnityEngine.Quaternion.identity, transform);
        }
        RegisterAttack("Area");
    }

    public void Summon()
    {
        int qtd = Random.Range(1, 2);
        List<Vector2> summonSpots = GetRandomArenaPoints(qtd, minDistance: 2f);
        foreach (Vector2 spot in summonSpots)
        {
            Instantiate(portalPrefab, spot, UnityEngine.Quaternion.identity, transform);
        }
        RegisterAttack("Summon");
    }
}

