using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public CardEffectData effectData;  // ✅ Reference to effect data

    public EffectType EffectType => effectData.effectType;  // ✅ Expose effectType
    public int EffectValue => effectData.baseValue;  // ✅ Expose base value

    public abstract void ApplyEffect(IEffectTarget target, int value);
}


