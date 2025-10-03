using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerAudioData", menuName = "Data/Player Audio Data")]
public class PlayerAudioData : ScriptableObject
{
    [Header("SFX")]
    public AudioClip[] normalAttacks;
    public AudioClip[] hitAttacks;
}
