using System.Collections;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Arraste o material branco que você criou aqui")]
    public Material whiteMaterial;
    [Tooltip("Duração do flash (0.1s é ideal para Moonlighter)")]
    public float duration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material = whiteMaterial;
        yield return new WaitForSeconds(duration);
        spriteRenderer.material = originalMaterial;

        flashRoutine = null;
    }
}