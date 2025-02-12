using UnityEngine;

[CreateAssetMenu(fileName = "ExhaustEffect", menuName = "Cards/Effects/Exhaust")]
public class ExhaustEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ReceiveEffect(value, EffectType.Exhaust);
    }
}
