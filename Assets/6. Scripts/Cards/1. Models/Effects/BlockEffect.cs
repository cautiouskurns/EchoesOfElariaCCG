using UnityEngine;

[CreateAssetMenu(fileName = "BlockEffect", menuName = "Cards/Effects/Block")]
public class BlockEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ApplyEffect(value, EffectType.Block);
    }
}
