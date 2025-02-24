using UnityEngine;

[CreateAssetMenu(fileName = "ExhaustedStatusEffect", menuName = "Status Effects/Exhausted")]
public class ExhaustedStatusEffect : BaseStatusEffect
{
    public override void ApplyStatus(IEffectTarget target, int duration)
    {
        if (target == null) return;
        Debug.Log($"[ExhaustedEffect] {target} is exhausted for {duration} turns!");

        //target.UseActionPoints(MaxAmount);  // Reduce available action points
    }

    public override void RemoveStatus(IEffectTarget target)
    {
        Debug.Log($"[ExhaustedEffect] Exhaustion lifted from {target}");
    }
}
