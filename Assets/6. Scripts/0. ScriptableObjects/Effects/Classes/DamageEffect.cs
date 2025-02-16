using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Effects/Damage Effect")]
public class DamageEffect : BaseEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        Debug.Log($"[DamageEffect] Applying {value} damage to {target}");
        target.ReceiveEffect(value, EffectType.Damage);
    }
}


