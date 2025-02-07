using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Cards/Effects/Damage")]
public class DamageEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ApplyEffect(value, EffectType.Damage);
    }
}
