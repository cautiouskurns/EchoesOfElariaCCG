using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Game/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string effectName;
    public Sprite effectIcon;
    public string description;
    public StatusType statusType;   // ✅ Uses the StatusType enum
    public int maxDuration;
    public AudioClip effectSound;
}