using UnityEngine;

[CreateAssetMenu(fileName = "StrengthEffect", menuName = "Cards/Effects/Strength")]
public class StrengthEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ReceiveEffect(value, EffectType.Strength);
    }
}
