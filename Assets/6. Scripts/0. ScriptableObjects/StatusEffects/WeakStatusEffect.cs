using UnityEngine;

[CreateAssetMenu(fileName = "New Weak Status Effect", menuName = "Status Effects/Weak")]
public class WeakStatusEffect : BaseStatusEffect
{
    public override void ApplyStatus(IEffectTarget target, int duration)
    {
        target.ReceiveStatusEffect(this, duration);
        Debug.Log($"{target} is now Weak for {duration} turns!");

        // Example Effect: Reduce target's attack damage by 25%
        if (target is BaseCharacter character)
        {
            character.ModifyStrength(-2); // Example: Reduce strength by 2
        }
    }

    public override void RemoveStatus(IEffectTarget target)
    {
        target.RemoveStatusEffect(this);
        Debug.Log($"{target} is no longer Weak!");

        // Example: Restore lost strength
        if (target is BaseCharacter character)
        {
            character.ModifyStrength(2); // Restore the strength that was lost
        }
    }
}
