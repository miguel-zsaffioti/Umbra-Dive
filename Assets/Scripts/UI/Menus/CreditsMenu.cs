using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{
    [Header("Fade In")]
    public CanvasGroup[] elements;      
    public float startDelay = 0.5f;     
    public float delayBetween = 0.6f;   
    public float fadeDuration = 1f;

    [Header("Cena")]
    public string menuSceneName = "MainMenu";

    private void Start()
    {
        Time.timeScale = 1f;

        foreach (var group in elements)
        {
            if (group == null) continue;
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        StartCoroutine(FadeInSequence());
    }

    private IEnumerator FadeInSequence()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        foreach (var group in elements)
        {
            if (group == null) continue;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }

            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;

            yield return new WaitForSecondsRealtime(delayBetween);
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}