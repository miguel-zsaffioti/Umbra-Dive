using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource;
    private Coroutine pendingPlay;
    private float baseVolume;

    public AudioClip menu;
    public AudioClip ambient;
    public AudioClip gameOver;

    [Header("Debug")]
    public bool logCalls = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource.volume;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Música/ambiente em loop
    public void Play(AudioClip clip)
    {
        Log("Play", clip);
        if (clip == null) return;

        CancelPending();

        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = baseVolume;
        audioSource.Play();
    }

    // Toca uma vez, sem loop, cortando o que estiver tocando. Devolve a duração do clipe.
    public float PlayOnce(AudioClip clip, float volumeScale = 1f)
    {
        Log("PlayOnce", clip);
        if (clip == null) return 0f;

        CancelPending();

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.volume = baseVolume * volumeScale;
        audioSource.Play();

        return clip.length;
    }

    // Espera o som atual (não-loop) terminar e então toca o clipe em loop
    public void PlayAfterCurrent(AudioClip clip)
    {
        Log("PlayAfterCurrent", clip);
        if (clip == null) return;

        CancelPending();
        pendingPlay = StartCoroutine(PlayAfterCurrentRoutine(clip));
    }

    private IEnumerator PlayAfterCurrentRoutine(AudioClip clip)
    {
        while (audioSource.isPlaying && !audioSource.loop)
        {
            yield return null;
        }

        pendingPlay = null;
        Play(clip);
    }

    public void Stop()
    {
        Log("Stop", null);
        CancelPending();
        audioSource.Stop();
    }

    private void CancelPending()
    {
        if (pendingPlay != null)
        {
            StopCoroutine(pendingPlay);
            pendingPlay = null;
        }
    }

    private void Log(string method, AudioClip clip)
    {
        if (!logCalls) return;
        Debug.Log($"[AudioManager] {method}({(clip != null ? clip.name : "null")})");
    }
}