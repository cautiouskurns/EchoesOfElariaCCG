using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Effect")]
public abstract class BaseEffect : ScriptableObject, IEffect  // ✅ Marked as abstract
{
    [SerializeField] private EffectType effectType;
    [SerializeField] private int baseValue;

    public EffectType EffectType => effectType;
    public int BaseValue => baseValue;

    public abstract void ApplyEffect(IEffectTarget target, int value);  // ✅ Abstract method
}
