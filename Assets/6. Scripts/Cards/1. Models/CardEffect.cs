using UnityEngine;

// ✅ Abstract class for all card effects
public abstract class CardEffect : ScriptableObject
{
    public abstract void ApplyEffect(IEffectTarget target, int value);
}


