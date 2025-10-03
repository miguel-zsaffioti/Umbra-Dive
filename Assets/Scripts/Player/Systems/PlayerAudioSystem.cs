using UnityEngine;

public class PlayerAudioSystem
{
    private Player player;
    private AudioSource audioSource;

    public PlayerAudioSystem(Player player)
    {
        this.player = player;
        this.audioSource = player.audioSource;
    }

    public void PlayRandomSound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
        {
            return;
        }

        AudioClip clipToPlay = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clipToPlay);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    //public void PlayFootstepSound()
    //{
    //    PlayRandomSound(player.audioData.footstepSounds);
    //}

    public void PlayAttackSound(bool hit)
    {
        Debug.Log("Chamou com hit igual a: " + hit);
        if (hit)
        {
            PlayRandomSound(player.audioData.hitAttacks);
            return;
        }
        

        PlayRandomSound(player.audioData.normalAttacks);
    }
}