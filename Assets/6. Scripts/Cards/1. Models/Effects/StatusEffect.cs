using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "Cards/Effects/Status")]
public class StatusEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ApplyEffect(value, EffectType.Status);
    }
}
