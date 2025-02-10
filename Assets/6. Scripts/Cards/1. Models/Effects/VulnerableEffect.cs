using UnityEngine;

[CreateAssetMenu(fileName = "VulnerableEffect", menuName = "Cards/Effects/Vulnerable")]
public class VulnerableEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        //target.ApplyEffect(value, EffectType.Vulnerable);
    }
}
