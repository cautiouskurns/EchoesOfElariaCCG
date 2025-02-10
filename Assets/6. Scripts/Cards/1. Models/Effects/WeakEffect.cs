using UnityEngine;

[CreateAssetMenu(fileName = "WeakEffect", menuName = "Cards/Effects/Weak")]
public class WeakEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        //target.ApplyEffect(value, EffectType.Weak);
    }
}
