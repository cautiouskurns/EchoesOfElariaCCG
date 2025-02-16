using UnityEngine;

[CreateAssetMenu(fileName = "New Poison Status Effect", menuName = "Status Effects/Poison")]
public class PoisonStatusEffect : BaseStatusEffect
{
    public override void ApplyStatus(IEffectTarget target, int duration)
    {
        target.ReceiveStatusEffect(this, duration);
        Debug.Log($"{target} is now Poisoned for {duration} turns!");

        // Example Effect: Reduce target's attack damage by 25%
        if (target is BaseCharacter character)
        {
            character.TakeDamage(MaxAmount); // Example: Reduce strength by 2
        }
    }

    public override void RemoveStatus(IEffectTarget target)
    {
        target.RemoveStatusEffect(this);
        Debug.Log($"{target} is no longer Poisoned!");
    }
}