using UnityEngine;

[CreateAssetMenu(fileName = "PowerEffect", menuName = "Cards/Effects/Power")]
public class PowerEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ApplyEffect(value, EffectType.Power);
    }
}
