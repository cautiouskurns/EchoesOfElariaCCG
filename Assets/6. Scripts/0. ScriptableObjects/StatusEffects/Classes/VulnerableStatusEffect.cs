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
        //t
        // 
        // arget.ModifyDefense(MaxAmount);  // Restore defense when removed
    }

    public override float GetDamageModifier()
    {
        // Use MaxAmount to determine the damage increase percentage
        // For Berserker's Rage, set MaxAmount to 50 (for 50%)
        return 1.0f + (MaxAmount / 100f); 
    }
}