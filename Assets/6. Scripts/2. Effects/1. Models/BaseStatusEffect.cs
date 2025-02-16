using UnityEngine;

public abstract class BaseStatusEffect : ScriptableObject, IStatusEffect
{
    public string EffectName;
    public StatusEffectTypes StatusType;
    public int MaxDuration;
    public Sprite Icon;
    public AudioClip EffectSound;

    public abstract void ApplyStatus(IEffectTarget target, int duration);
    public abstract void RemoveStatus(IEffectTarget target);
}

