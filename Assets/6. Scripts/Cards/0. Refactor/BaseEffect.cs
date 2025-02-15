using UnityEngine;

public abstract class BaseEffect : ScriptableObject, IEffect
{
    [SerializeField] private EffectType effectType;
    [SerializeField] private int baseValue;

    public EffectType EffectType => effectType;
    public int BaseValue => baseValue;

    public virtual void ApplyEffect(IEffectTarget target, int value)
    {
        Debug.Log($"Applying {effectType} effect with value {value} to {target}.");
    }
}
