using System.Collections;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public int flickerCount = 6;
    public float startFlickerInterval = 0.35f;
    public float endFlickerInterval = 0.04f;
    public GameObject deathParticlesPrefab;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    public Material whiteMaterial;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public IEnumerator PlayDeathEffect()
    {
        for (int i = 0; i < flickerCount; i++)
        {
            float t = flickerCount > 1 ? (float)i / (flickerCount - 1) : 0f;
            float interval = Mathf.Lerp(startFlickerInterval, endFlickerInterval, t);

            spriteRenderer.material = whiteMaterial;
            yield return new WaitForSeconds(interval);

            spriteRenderer.material = originalMaterial;
            yield return new WaitForSeconds(interval);
        }

        SpawnParticlesAndHide();
    }

    public IEnumerator PlayDeathEffectInstant()
    {
        SpawnParticlesAndHide();
        yield break;
    }

    private void SpawnParticlesAndHide()
    {
        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }

        spriteRenderer.enabled = false;
    }
}