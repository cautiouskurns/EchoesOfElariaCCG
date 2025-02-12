using UnityEngine;

public abstract class BaseStatusEffect : ScriptableObject, IStatusEffect
{
    public string EffectName;
    public StatusType StatusType;
    public int MaxDuration;
    public Sprite Icon;
    public AudioClip EffectSound;

    public virtual void ApplyStatus(IEffectTarget target, int duration)
    {
        target.ReceiveStatusEffect(this, duration);
        Debug.Log($"{target} gained {EffectName} for {duration} turns.");
    }

    public virtual void RemoveStatus(IEffectTarget target)
    {
        target.RemoveStatusEffect(this);
        Debug.Log($"{target} lost {EffectName}.");
    }
}

