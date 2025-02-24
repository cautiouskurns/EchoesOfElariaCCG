using UnityEngine;

public interface IEffectTarget
{
    void ReceiveEffect(int value, EffectType type);
    void ReceiveStatusEffect(IStatusEffect effect, int duration);
    void RemoveStatusEffect(IStatusEffect effect);
    bool HasStatusEffect(StatusEffectTypes statusType);
    bool HasBuff();
    bool HasDebuff();
}
