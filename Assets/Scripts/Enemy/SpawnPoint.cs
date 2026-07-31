using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] prefabs;

    [Range(0f, 1f)]
    public float spawnChance = 0.5f;

    private void Awake()
    {
        foreach (Transform spawnPoint in transform)
        {
            if (Random.value > spawnChance) continue;

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}