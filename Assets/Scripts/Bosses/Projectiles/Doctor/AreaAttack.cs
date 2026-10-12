using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AreaAttack : MonoBehaviour, IPoolable
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color baseColor;

    private bool inCooldown = true;

    public int damage = 5;
    public int maxDamageTicks = 5;
    private int currentTick = 0;

    public float cooldown = 1.5f;

    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;

    private bool hasDamagedThisActivation = false;
    private bool activated = false;

    // Som em loop (AudioSource criado só quando o ataque pede o som)
    private AudioSource loopSource;
    private float loopVolume = 1f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        baseColor = spriteRenderer.color; // cor inicial do prefab (alpha incluso)
    }

    public void OnSpawn()
    {
        inCooldown = true;
        currentTick = 0;
        activated = false;
        hasDamagedThisActivation = false;
        spriteRenderer.color = baseColor;

        StartCoroutine(FadeIn(fadeInDuration));
    }

    public void OnDespawn()
    {
        StopAllCoroutines();
        activated = false;

        if (loopSource != null)
        {
            loopSource.Stop();
        }
    }

    // Chamado pelo Doctor logo depois do Spawn
    public void PlayLoop(AudioClip clip, float volume, AudioMixerGroup mixerGroup)
    {
        if (clip == null) return;

        if (loopSource == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.playOnAwake = false;
            loopSource.loop = true;
            loopSource.spatialBlend = 0f;
        }

        loopVolume = volume;
        loopSource.outputAudioMixerGroup = mixerGroup;
        loopSource.clip = clip;
        loopSource.volume = loopVolume * spriteRenderer.color.a;
        loopSource.Play();
    }

    private void SetLoopVolumeByAlpha(float alpha)
    {
        if (loopSource != null)
        {
            loopSource.volume = loopVolume * alpha;
        }
    }

    private void Update()
    {
        if (inCooldown) return;
        if (currentTick >= maxDamageTicks)
        {
            StartCoroutine(FadeOut(fadeOutDuration));
            return;
        }
        animator.SetTrigger("Damage");

        currentTick++;
        StartCoroutine(DamageCooldown());
    }

    private IEnumerator DamageCooldown()
    {
        inCooldown = true;
        hasDamagedThisActivation = false;
        yield return new WaitForSecondsRealtime(cooldown);
        inCooldown = false;
    }

    private IEnumerator FadeIn(float duration)
    {
        Color color = spriteRenderer.color;

        while (color.a < 1f)
        {
            color.a += Time.deltaTime / duration;
            spriteRenderer.color = color;
            SetLoopVolumeByAlpha(color.a);
            yield return null;
        }

        color.a = 1f;
        spriteRenderer.color = color;
        SetLoopVolumeByAlpha(1f);
        inCooldown = false;
    }

    private IEnumerator FadeOut(float duration)
    {
        inCooldown = true;
        animator.SetTrigger("Fade");
        Color color = spriteRenderer.color;

        while (color.a > 0f)
        {
            color.a -= Time.deltaTime / duration;
            spriteRenderer.color = color;
            SetLoopVolumeByAlpha(color.a);
            yield return null;
        }

        color.a = 0f;
        spriteRenderer.color = color;
        PoolManager.Instance.Despawn(gameObject);
    }

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!activated || hasDamagedThisActivation) return;
        if (collision.CompareTag("Player"))
        {
            IDamagable damageable = collision.gameObject.GetComponent<IDamagable>();
            if (damageable != null)
            {
                hasDamagedThisActivation = true;
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                damageable.TakeDamage(damage, knockbackDirection, 0f);
            }
        }
    }
}