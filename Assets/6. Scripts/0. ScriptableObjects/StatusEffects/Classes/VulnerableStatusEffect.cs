using UnityEngine;

[CreateAssetMenu(fileName = "VulnerableStatusEffect", menuName = "Status Effects/Vulnerable")]
public class VulnerableEffect : BaseStatusEffect
{
    public override void ApplyStatus(IEffectTarget target, int duration)
    {
        if (target == null) return;
        Debug.Log($"[VulnerableEffect] {target} is vulnerable for {duration} turns!");

        //target.ModifyDefense(-MaxAmount);  // Reduce defense temporarily
    }

    public override void RemoveStatus(IEffectTarget target)
    {
        Debug.Log($"[VulnerableEffect] Vulnerability removed from {target}");
        //target.ModifyDefense(MaxAmount);  // Restore defense when removed
    }
}