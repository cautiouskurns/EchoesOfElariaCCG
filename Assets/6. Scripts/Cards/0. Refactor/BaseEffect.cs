using UnityEngine;

public abstract class BaseEffect : ScriptableObject, IEffect
{
    [SerializeField] private EffectType effectType;
    [SerializeField] private int baseValue;

    public EffectType EffectType => effectType;
    public int BaseValue => baseValue;

    public abstract void ApplyEffect(IEffectTarget target, int value);
}
