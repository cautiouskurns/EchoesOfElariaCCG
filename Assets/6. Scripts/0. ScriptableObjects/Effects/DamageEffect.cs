using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Effects/Damage")]
public class DamageEffect : BaseEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ReceiveEffect(value, EffectType.Damage);
        Debug.Log($"[DamageEffect] Applied {value} damage to {target}.");
    }
}

