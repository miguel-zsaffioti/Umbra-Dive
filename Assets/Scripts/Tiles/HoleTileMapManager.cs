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
        holeTilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;

            Vector3Int cellPosition = holeTilemap.WorldToCell(player.transform.position);

            Vector3 fallCenter = holeTilemap.GetCellCenterWorld(cellPosition);

            StartCoroutine(FallSequence(player, fallCenter));
        }
    }

    private IEnumerator FallSequence(GameObject player, Vector3 fallCenter)
    {
        player.GetComponent<Collider2D>().enabled = false;

        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite == null) yield break;

        playerSprite.sortingOrder = -1;

        Vector3 originalScale = player.transform.localScale;
        Vector3 startPosition = player.transform.position;
        Color originalColor = playerSprite.color;
        float timer = 0f;

        while (timer < fallDuration)
        {
            float progress = timer / fallDuration;

            player.transform.position = Vector3.Lerp(startPosition, fallCenter, progress);
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
            playerSprite.color = Color.Lerp(originalColor, Color.clear, progress);

            timer += Time.deltaTime;
            yield return null;
        }

        player.SetActive(false);

        yield return new WaitForSeconds(1f);

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.LoadCheckpoint();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}