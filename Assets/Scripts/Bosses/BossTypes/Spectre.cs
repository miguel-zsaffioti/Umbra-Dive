using UnityEngine;

public class Spectre : Boss
{
    [SerializeField] GameObject OreShard;

    public void SpawnProjectile(Vector2 direction, Quaternion rotation)
    {
        Vector2 spawnLocation = rb.position + direction;
        Instantiate(OreShard, spawnLocation, rotation);
    }

    public void SpawnProjectileInCrossPattern()
    {
        // Lógica futura
    }

    public void SpawnProjectileInXPattern()
    {
        // Lógica futura
    }
}