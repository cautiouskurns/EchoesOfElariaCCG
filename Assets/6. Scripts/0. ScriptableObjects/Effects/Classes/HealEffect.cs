using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Effect", menuName = "Effects/Heal")]
public class HealEffect : BaseEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ReceiveEffect(value, EffectType.Heal);
        Debug.Log($"[HealEffect] Restored {value} HP to {target}.");
    }
}

