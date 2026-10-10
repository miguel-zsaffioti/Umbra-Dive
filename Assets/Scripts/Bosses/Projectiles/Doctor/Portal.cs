using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour, IPoolable
{
    public float spawnCooldown = 3f;
    public GameObject slimePrefab;
    public GameObject batPrefab;

    private Animator animator;
    private Vector3 baseScale;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        baseScale = transform.localScale; // escala original do prefab
    }

    public void OnSpawn()
    {
        transform.localScale = baseScale;

        StartCoroutine(Appear());
        StartCoroutine(SpawnWait());
    }

    public void OnDespawn()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnWait()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float clipLength = clipInfo.Length > 0 ? clipInfo[0].clip.length : 1f;
        yield return new WaitForSeconds(clipLength * spawnCooldown);
        Spawn();
    }

    private void Spawn()
    {
        int rand = Random.Range(0, 100);
        GameObject enemyObj = Instantiate(rand > 50 ? slimePrefab : batPrefab, transform.position, Quaternion.identity);

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ForceChase();
        }

        PoolManager.Instance.Despawn(gameObject);
    }

    private IEnumerator Collapse()
    {
        float duration = 0.4f;
        float t = 0f;
        Vector3 startScale = transform.localScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, p);
            transform.Rotate(0, 0, 720f * Time.deltaTime);
            yield return null;
        }

        PoolManager.Instance.Despawn(gameObject);
    }

    private IEnumerator Appear()
    {
        float duration = 0.4f;
        float t = 0f;
        Vector3 targetScale = baseScale;
        Quaternion targetRotation = transform.rotation;
        transform.localScale = Vector3.zero;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, p);
            transform.Rotate(0, 0, 720f * (1f - p) * Time.deltaTime);
            yield return null;
        }

        transform.localScale = targetScale;
        transform.rotation = targetRotation;
    }
}