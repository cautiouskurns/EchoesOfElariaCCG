using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Cards/Effects/Heal")]
public class HealEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ApplyEffect(value, EffectType.Heal);
    }
}
