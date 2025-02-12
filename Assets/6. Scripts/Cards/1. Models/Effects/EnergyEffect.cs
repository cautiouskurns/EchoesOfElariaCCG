using UnityEngine;

[CreateAssetMenu(fileName = "EnergyEffect", menuName = "Cards/Effects/Energy")]
public class EnergyEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ReceiveEffect(value, EffectType.Energy);
    }
}
