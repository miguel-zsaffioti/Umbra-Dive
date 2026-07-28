using UnityEngine;

public class Spectre : Boss
{
    [SerializeField] GameObject OreShard;

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

    public void SpawnProjectile(Vector2 direction)
    {
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
        foreach (Vector2 direction in CrossDirections)
        {
            SpawnProjectile(direction);
        }
    }

    public void SpawnProjectileInXPattern()
    {
        foreach (Vector2 direction in XDirections)
        {
            SpawnProjectile(direction);
        }
    }
}