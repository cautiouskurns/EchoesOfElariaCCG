using UnityEngine;

[CreateAssetMenu(fileName = "DestabilizedStatusEffect", menuName = "Status Effects/Destabilized")]
public class DestabilizedStatusEffect : BaseStatusEffect
{
    public override void ApplyStatus(IEffectTarget target, int duration)
    {
        if (target == null) return;
        Debug.Log($"[DestabilizedEffect] {target} is destabilized for {duration} turns!");

        //target.ModifyLuck(-MaxAmount);  // Lower luck stat, increasing chance of critical hits received
    }

    public override void RemoveStatus(IEffectTarget target)
    {
        Debug.Log($"[DestabilizedEffect] Stabilization restored for {target}");
        //target.ModifyLuck(MaxAmount);  // Restore luck after effect ends
    }
}
