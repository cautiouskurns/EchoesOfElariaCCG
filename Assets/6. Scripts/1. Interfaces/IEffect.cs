using UnityEngine;

public interface IEffect
{
    void ApplyEffect(IEffectTarget target, int value);
}
