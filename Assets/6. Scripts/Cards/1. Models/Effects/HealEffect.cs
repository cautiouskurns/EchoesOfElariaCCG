using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Cards/Effects/Heal")]
public class HealEffect : CardEffect
{
    public override void ApplyEffect(BaseCharacter target, int value)
    {
        target.Heal(value);
    }
}

