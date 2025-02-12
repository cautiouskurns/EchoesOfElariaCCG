using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Effects/Damage")]
public class TestEffect : BaseEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ReceiveEffect(value, EffectType.Damage);
    }
}
