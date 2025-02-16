using UnityEngine;

[CreateAssetMenu(fileName = "New Strengthen Status Effect", menuName = "Status Effects/Strengthened")]
public class StrengthenedStatusEffect : BaseStatusEffect
{
    public override void ApplyStatus(IEffectTarget target, int duration)
    {
        target.ReceiveStatusEffect(this, duration);
        Debug.Log($"{target} is now Strengthened for {duration} turns!");

        // Example Effect: Reduce target's attack damage by 25%
        if (target is BaseCharacter character)
        {
            character.ModifyStrength(MaxAmount); // Example: Reduce strength by 2
        }
    }

    public override void RemoveStatus(IEffectTarget target)
    {
        target.RemoveStatusEffect(this);
        Debug.Log($"{target} is no longer Strengthened!");

        // Example: Restore lost strength
        if (target is BaseCharacter character)
        {
            character.ModifyStrength(MaxAmount); // Restore the strength that was lost
        }
    }
}
