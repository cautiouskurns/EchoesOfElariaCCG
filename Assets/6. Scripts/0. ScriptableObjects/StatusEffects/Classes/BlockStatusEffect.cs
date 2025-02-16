using UnityEngine;

[CreateAssetMenu(fileName = "New Block Status Effect", menuName = "Status Effects/Block")]
public class BlockStatusEffect : BaseStatusEffect
{
    public override void ApplyStatus(IEffectTarget target, int duration)
    {
        target.ReceiveStatusEffect(this, duration);
        Debug.Log($"{target} is now Blocking for {duration} turns!");

        // Example Effect: Reduce target's attack damage by 25%
        if (target is BaseCharacter character)
        {
            character.GainBlock(MaxAmount); // Example: Reduce strength by 2
        }
    }

    public override void RemoveStatus(IEffectTarget target)
    {
        target.RemoveStatusEffect(this);
        Debug.Log($"{target} is no longer Blocking!");

        // Example: Restore lost strength
        if (target is BaseCharacter character)
        {
            character.GainBlock(-MaxAmount); // Restore the strength that was lost
        }
    }
}