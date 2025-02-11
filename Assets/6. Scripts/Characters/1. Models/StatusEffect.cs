using UnityEngine;

public enum StatusType
{
    Weak,
    Vulnerable,
    Poison,
    StrengthBuff,
    Destabilized,
    Exhausted
}

public class StatusEffect
{
    public StatusType Type { get; private set; }  
    public int Duration { get; set; }     
    public StatusEffectData EffectData { get; private set; }  // ✅ Added reference to effect data

    public StatusEffect(StatusType type, int duration, StatusEffectData effectData)
    {
        this.Type = type;
        this.Duration = duration;
        this.EffectData = effectData;  // ✅ Store effect data reference
    }

    public void ReduceDuration()
    {
        Duration--;
    }

    public bool IsExpired()
    {
        return Duration <= 0;
    }
}
