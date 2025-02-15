using UnityEngine;

[CreateAssetMenu(fileName = "New Weak Effect", menuName = "Effects/Status Effects/Weak")]
public class WeakStatusEffect : BaseStatusEffect
{
    public override void ApplyStatus(IEffectTarget target, int duration)
    {
        base.ApplyStatus(target, duration);
        Debug.Log($"[WeakStatusEffect] {target} is Weak for {duration} turns!");
    }

    public override void RemoveStatus(IEffectTarget target)
    {
        base.RemoveStatus(target);
        Debug.Log($"[WeakStatusEffect] {target} is no longer poisoned.");
    }
}
