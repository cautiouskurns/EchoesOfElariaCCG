using UnityEngine;

[CreateAssetMenu(fileName = "New Card Effect", menuName = "Game/Card Effect")]
public class CardEffectData : ScriptableObject
{
    public string effectName;
    public EffectType effectType;  // ✅ Now correctly references effect type
    public int baseValue;          // Default value of the effect (e.g., 8 for Damage)
    public AudioClip effectSound;  // ✅ Sound effect when applied

    [Header("Status Effect (Optional)")]
    public StatusEffectData statusEffectData;  // ✅ Added reference to status effects

    [Header("UI Representation")]
    public Sprite effectIcon;
}
