using System.Collections.Generic;
using UnityEngine;

public class Doctor : Boss
{
    public AreaSlash areaSlashPrefab;
    public FrontAttack frontAttackPrefab;
    public AreaAttack areaAttackPrefab;
    public Portal portalPrefab;
    public Projectile projectilePrefab;
    public float frontAttackSpawnDistance = 1.2f;
    public float cornerMargin = 0.6f;

    public float projectileSpawnDistance = 1.2f;

    [HideInInspector] public Vector2 arenaCenter;

    private System.Action pendingAttack;

    [HideInInspector] public Vector2 frontAttackDirection;
    [HideInInspector] public Vector2 frontAttackTargetPos;

    [HideInInspector] public List<Vector2> arenaCorners;

    [HideInInspector] public AudioSource audioSource;
    public AudioClip areaSlash;
    public AudioClip dash;
    public AudioClip frontAttack;
    public AudioClip projectile;
    public AudioClip area;
    [Range(0f, 1f)] public float areaVolume = 0.6f;
    public AudioClip portal;

    public enum AttackTypes
    {
        Dash,
        AreaSlash,
        Front,
        Area,
        Summon,
        Projectile
    }

    protected override void Start()
    {
        base.Start();
        arenaCenter = arenaBounds.bounds.center;
        arenaCorners = GetArenaCorners();
        audioSource = GetComponent<AudioSource>();
    }

    private List<Vector2> GetArenaCorners()
    {
        Bounds bounds = arenaBounds.bounds;

        return new List<Vector2>
        {
            new Vector2(bounds.min.x + cornerMargin, bounds.min.y + cornerMargin),
            new Vector2(bounds.max.x - cornerMargin, bounds.min.y + cornerMargin),
            new Vector2(bounds.max.x - cornerMargin, bounds.max.y - cornerMargin),
            new Vector2(bounds.min.x + cornerMargin, bounds.max.y - cornerMargin)
        };
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

    public void PlayPortalSound()
    {
        if (audioSource == null || portal == null) return;

        audioSource.PlayOneShot(portal);
    }

    public void AreaAttack()
    {
        if (areaSlashPrefab == null) return;
        SetAttackDirection(Vector2.down);
        PoolManager.Instance.Spawn(areaSlashPrefab, transform.position, transform.rotation, transform);
        audioSource.PlayOneShot(areaSlash);
        RegisterAttack("AreaSlash");
    }

    public void Dash()
    {
        audioSource.PlayOneShot(dash);
        RegisterAttack("Dash");
    }

    public void FrontAttack()
    {
        float angle = Mathf.Atan2(frontAttackDirection.y, frontAttackDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        float distToTarget = Vector2.Distance(transform.position, frontAttackTargetPos);
        float spawnDist = Mathf.Min(frontAttackSpawnDistance, distToTarget * 0.8f);

        Vector3 spawnPos = transform.position + (Vector3)(frontAttackDirection * spawnDist);

        PoolManager.Instance.Spawn(frontAttackPrefab, spawnPos, rotation, transform);
        audioSource.PlayOneShot(frontAttack);
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

        for (int i = 0; i < poisonSpots.Count; i++)
        {
            AreaAttack areaAttack = PoolManager.Instance.Spawn(areaAttackPrefab, poisonSpots[i], Quaternion.identity, transform);

            if (i == 0)
            {
                var mixerGroup = audioSource != null ? audioSource.outputAudioMixerGroup : null;
                areaAttack.PlayLoop(area, areaVolume, mixerGroup);
            }
        }

        RegisterAttack("Area");
    }

    public void Summon()
    {
        int qtd = Random.Range(1, 2);
        List<Vector2> summonSpots = GetRandomArenaPoints(qtd, minDistance: 2f);
        foreach (Vector2 spot in summonSpots)
        {
            PoolManager.Instance.Spawn(portalPrefab, spot, Quaternion.identity, transform);
        }
        RegisterAttack("Summon");
    }

    public void Projectile()
    {
        Vector2 nearestCorner = GetNearestCorner(out int cornerIndex);
        Vector2 animationDir = Vector2.zero;
        Vector2 spawnDir = Vector2.zero;

        switch (cornerIndex)
        {
            case 0:
                animationDir = Vector2.right;
                spawnDir = Vector2.up + animationDir;
                break;
            case 1:
                animationDir = Vector2.left;
                spawnDir = Vector2.up + animationDir;
                break;
            case 2:
                animationDir = Vector2.left;
                spawnDir = Vector2.down + animationDir;
                break;
            case 3:
                animationDir = Vector2.right;
                spawnDir = Vector2.down + animationDir;
                break;
        }

        Vector3 spawnPos = transform.position + (Vector3)(spawnDir.normalized * projectileSpawnDistance);
        PoolManager.Instance.Spawn(projectilePrefab, spawnPos, Quaternion.identity);
        audioSource.PlayOneShot(projectile);
        RegisterAttack("Projectile");
    }

    public Vector2 GetNearestCorner(out int cornerIndex)
    {
        cornerIndex = -1;
        Vector2 nearest = transform.position;

        if (arenaCorners == null || arenaCorners.Count == 0) return nearest;

        Vector2 bossPos = transform.position;
        float bestDist = float.MaxValue;

        for (int i = 0; i < arenaCorners.Count; i++)
        {
            float dist = Vector2.Distance(bossPos, arenaCorners[i]);
            if (dist < bestDist)
            {
                bestDist = dist;
                nearest = arenaCorners[i];
                cornerIndex = i;
            }
        }

        return nearest;
    }
}