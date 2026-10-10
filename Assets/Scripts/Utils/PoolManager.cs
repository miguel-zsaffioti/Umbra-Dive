using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 200;

    private readonly Dictionary<GameObject, IObjectPool<GameObject>> pools = new();
    private readonly Dictionary<GameObject, GameObject> instanceToPrefab = new();
    private readonly Dictionary<GameObject, Transform> containers = new();

    // Instâncias que estão em uso agora (fora da pool)
    private readonly HashSet<GameObject> active = new();
    private readonly List<GameObject> despawnBuffer = new();

    private static readonly List<IPoolable> poolableBuffer = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Prewarm(GameObject prefab, int count)
    {
        var pool = GetOrCreatePool(prefab);
        var temp = new List<GameObject>(count);

        for (int i = 0; i < count; i++) temp.Add(pool.Get());
        foreach (var go in temp) pool.Release(go);
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var pool = GetOrCreatePool(prefab);
        var instance = pool.Get();

        if (parent != null)
            instance.transform.SetParent(parent, false);

        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        active.Add(instance);

        instance.GetComponents(poolableBuffer);
        foreach (var p in poolableBuffer) p.OnSpawn();

        return instance;
    }

    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
    {
        var go = Spawn(prefab.gameObject, position, rotation, parent);
        return go.GetComponent<T>();
    }

    public void Despawn(GameObject instance)
    {
        if (instance == null) return;

        if (!instanceToPrefab.TryGetValue(instance, out var prefab))
        {
            Debug.LogWarning($"[PoolManager] '{instance.name}' não pertence a nenhum pool. Destruindo.", instance);
            Destroy(instance);
            return;
        }

        pools[prefab].Release(instance);
    }

    public void DespawnAll()
    {
        despawnBuffer.Clear();
        despawnBuffer.AddRange(active);

        foreach (var instance in despawnBuffer)
        {
            if (instance != null) Despawn(instance);
        }

        despawnBuffer.Clear();
    }

    private IObjectPool<GameObject> GetOrCreatePool(GameObject prefab)
    {
        if (pools.TryGetValue(prefab, out var existing)) return existing;

        var container = new GameObject($"Pool_{prefab.name}").transform;
        container.SetParent(transform);
        containers[prefab] = container;

        var pool = new ObjectPool<GameObject>(
            createFunc: () => CreateInstance(prefab),
            actionOnGet: null,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyInstance,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        pools[prefab] = pool;
        return pool;
    }

    private GameObject CreateInstance(GameObject prefab)
    {
        var instance = Instantiate(prefab, containers[prefab]);
        instance.SetActive(false);
        instanceToPrefab[instance] = prefab;
        return instance;
    }

    private void OnRelease(GameObject instance)
    {
        active.Remove(instance);

        instance.GetComponents(poolableBuffer);
        foreach (var p in poolableBuffer) p.OnDespawn();

        instance.SetActive(false);
        instance.transform.SetParent(containers[instanceToPrefab[instance]], false);
    }

    private void OnDestroyInstance(GameObject instance)
    {
        active.Remove(instance);
        instanceToPrefab.Remove(instance);
        Destroy(instance);
    }
}