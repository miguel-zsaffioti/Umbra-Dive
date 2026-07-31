using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class Spectre : Boss
{
    [SerializeField] GameObject OreShard;
    [SerializeField] GameObject teleportEffectPrefab;

    public void ForceFaceForward(Vector2? direction = null)
    {
        Vector2 facing = direction ?? Vector2.down;
        animator.SetBool("isMoving", false);
        animator.SetFloat("LastX", facing.x);
        animator.SetFloat("LastY", facing.y);
    }

    private static readonly Vector2[] CrossDirections =
    {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };

    private static readonly Vector2[] XDirections =
    {
        new Vector2(1f, 1f).normalized,
        new Vector2(-1f, 1f).normalized,
        new Vector2(1f, -1f).normalized,
        new Vector2(-1f, -1f).normalized
    };

    public void PlayTeleportEffect(Vector2 position)
    {
        if (teleportEffectPrefab == null) return;
        GameObject fx = Instantiate(teleportEffectPrefab, position, Quaternion.identity);
        Destroy(fx, 2f);
    }

    private void Update()
    {
        Vector2 moveDir = moveDirection.normalized;
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;
        //Debug.Log(isMoving);

        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("LastX", moveDir.x);
            animator.SetFloat("LastY", moveDir.y);
        }
    }

    private void SpawnProjectile(Vector2 direction)
    {
        animator.SetTrigger("Attack");
        Vector2 spawnLocation = rb.position + direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject shard = Instantiate(OreShard, spawnLocation, rotation);

        if (direction.x < 0f)
        {
            Vector3 scale = shard.transform.localScale;
            scale.y *= -1f;
            shard.transform.localScale = scale;
        }
    }

    public void SpawnProjectileInCrossPattern()
    {
        animator.SetTrigger("Attack");
        foreach (Vector2 direction in CrossDirections)
        {
            SpawnProjectile(direction);
        }
    }

    public void SpawnProjectileInXPattern()
    {
        animator.SetTrigger("Attack");
        foreach (Vector2 direction in XDirections)
        {
            SpawnProjectile(direction);
        }
    }

    public void SpawnProjectileTeleport()
    {
        animator.SetTrigger("Attack");
        Vector2 directionToPlayer = (player.rb.position - rb.position).normalized;
        SpawnProjectile(directionToPlayer);
    }
}