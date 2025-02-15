using UnityEngine;

public interface IStatusEffect
{
    void ApplyStatus(IEffectTarget target, int duration);
    void RemoveStatus(IEffectTarget target);
}
