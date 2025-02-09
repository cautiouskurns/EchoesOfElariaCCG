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

public class StatusEffects
{
    public StatusType Type;
    public int Duration;

    public StatusEffects(StatusType type, int duration)
    {
        Type = type;
        Duration = duration;
    }
}
