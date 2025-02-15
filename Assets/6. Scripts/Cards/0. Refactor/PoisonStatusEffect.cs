using UnityEngine;

[CreateAssetMenu(fileName = "New Poison Effect", menuName = "Effects/Status Effects/Poison")]
public class PoisonEffect : BaseStatusEffect
{
    public override void ApplyStatus(IEffectTarget target, int duration)
    {
        base.ApplyStatus(target, duration);
        Debug.Log($"[PoisonEffect] {target} is poisoned for {duration} turns!");
    }

    public override void RemoveStatus(IEffectTarget target)
    {
        base.RemoveStatus(target);
        Debug.Log($"[PoisonEffect] {target} is no longer poisoned.");
    }
}