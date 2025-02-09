using UnityEngine;

[CreateAssetMenu(fileName = "EnergyEffect", menuName = "Cards/Effects/Energy")]
public class EnergyEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ApplyEffect(value, EffectType.Energy);
    }
}
