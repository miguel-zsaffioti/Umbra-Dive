using System;
using System.Collections;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool inCooldown = true;

    public int damage = 5;
    public int maxDamageTicks = 5;
    private int currentTick = 0;

    public float cooldown = 1.5f;
    
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;

    private bool hasDamagedThisActivation = false;
    private bool activated = false;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(FadeIn(fadeInDuration));
        currentTick = 0;
    }

    void Update()
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
            yield return null;
        }

        color.a = 1f;
        spriteRenderer.color = color;
        inCooldown= false;
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
            yield return null;
        }

        color.a = 0f;
        spriteRenderer.color = color;
        Destroy(gameObject);
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
        Debug.Log("Teste");
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
