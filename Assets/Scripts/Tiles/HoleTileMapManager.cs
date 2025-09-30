using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement; // Opcional

[RequireComponent(typeof(Tilemap))]
public class HoleTilemapManager : MonoBehaviour
{
    [Header("Configurações do Efeito")]
    [Tooltip("Quanto tempo dura a animação de queda em segundos.")]
    public float fallDuration = 0.5f;

    private Tilemap holeTilemap;

    private void Awake()
    {
        // Pega a referência do Tilemap ao qual este script está anexado.
        holeTilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Pega o objeto do jogador
            GameObject player = other.gameObject;

            // --- A MÁGICA ACONTECE AQUI ---
            // 1. Converte a posição do jogador (world space) para a posição da célula do tilemap (cell space)
            Vector3Int cellPosition = holeTilemap.WorldToCell(player.transform.position);

            // 2. Pega a posição central exata daquela célula no mundo
            Vector3 fallCenter = holeTilemap.GetCellCenterWorld(cellPosition);

            // Inicia a rotina da queda, passando a posição central do tile como referência
            StartCoroutine(FallSequence(player, fallCenter));
        }
    }

    private IEnumerator FallSequence(GameObject player, Vector3 fallCenter)
    {
        // Desativa o collider do jogador para não acionar o trigger novamente
        player.GetComponent<Collider2D>().enabled = false;

        // Desativa o controle do jogador (substitua 'PlayerMovement' pelo nome do seu script de movimento)
        // Exemplo: player.GetComponent<PlayerMovement>().enabled = false;

        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite == null) yield break;

        // Detalhe Isométrico: Faz o jogador renderizar atrás do cenário
        playerSprite.sortingOrder = -1;

        Vector3 originalScale = player.transform.localScale;
        Vector3 startPosition = player.transform.position;
        Color originalColor = playerSprite.color;
        float timer = 0f;

        while (timer < fallDuration)
        {
            float progress = timer / fallDuration;

            // Move o personagem suavemente para o centro do buraco enquanto cai
            player.transform.position = Vector3.Lerp(startPosition, fallCenter, progress);

            // Encolhe o personagem
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);

            // Esmaece (fade out) o personagem
            playerSprite.color = Color.Lerp(originalColor, Color.clear, progress);

            timer += Time.deltaTime;
            yield return null;
        }

        player.SetActive(false);

        // Exemplo: Reiniciar a cena atual após 1 segundo
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}