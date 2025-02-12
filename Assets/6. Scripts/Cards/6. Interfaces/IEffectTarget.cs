// public interface IEffectTarget
// {
//     void ReceiveEffect(int value, EffectType type); // This method will be called by the effect to apply the effect to the target
// }

using UnityEngine;

public interface IEffectTarget
{
    void ReceiveEffect(int value, EffectType type); // General effect application (e.g., damage, healing)
    void ReceiveStatusEffect(IStatusEffect effect, int duration); // Handles status effects
    void RemoveStatusEffect(IStatusEffect effect); // Removes status effects
}
