using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewPlayerAudioData", menuName = "Data/Player Audio Data")]
public class PlayerAudioData : ScriptableObject
{
    public AudioResource footsteps;
    public AudioResource shield;
    public AudioResource swordSlash;
    public AudioResource swordImpact;
    public AudioResource hit;
    public AudioResource potionUse;
    public AudioResource dodge;
}
