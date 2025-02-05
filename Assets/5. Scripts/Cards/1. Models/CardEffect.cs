using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void ApplyEffect(BaseCharacter target, int value);
}

