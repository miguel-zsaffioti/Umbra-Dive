using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class DeathEffect : MonoBehaviour
{
    public int flickerCount = 6;
    public float startFlickerInterval = 0.35f;
    public float endFlickerInterval = 0.04f;
    public GameObject deathParticlesPrefab;

    [Header("Audio (opcional, só usado na morte de boss)")]
    public AudioClip buildUpSound;
    [Range(0f, 1f)] public float buildUpVolume = 0.6f;
    public AudioClip explosionSound;
    [Range(0f, 1f)] public float explosionVolume = 0.6f;

    [Header("Camera Shake (opcional, só usado na morte de boss)")]
    public float shakeForce = 1f;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    public Material whiteMaterial;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public IEnumerator PlayDeathEffect()
    {
        // Para a música da luta e toca o build-up
        if (AudioManager.Instance != null && buildUpSound != null)
        {
            AudioManager.Instance.PlayOnce(buildUpSound, buildUpVolume);
        }

        for (int i = 0; i < flickerCount; i++)
        {
            float t = flickerCount > 1 ? (float)i / (flickerCount - 1) : 0f;
            float interval = Mathf.Lerp(startFlickerInterval, endFlickerInterval, t);

            spriteRenderer.material = whiteMaterial;
            yield return new WaitForSeconds(interval);

            spriteRenderer.material = originalMaterial;
            yield return new WaitForSeconds(interval);
        }

        if (AudioManager.Instance != null && explosionSound != null)
        {
            AudioManager.Instance.PlayOnce(explosionSound, explosionVolume);
        }

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulseWithForce(shakeForce);
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