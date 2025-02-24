using UnityEngine;
using Cards;

public static class ConditionEvaluator
{
    /// <summary>
    /// Checks if a condition is met for a given target.
    /// </summary>
    public static bool IsConditionMet(ConditionType condition, BaseCharacter target, int conditionValue = 0)
    {
        switch (condition)
        {
            case ConditionType.None:
                return true; // Always applies

            case ConditionType.LastCardWasAttack:
                return CardManager.Instance.LastCardPlayed.CardType == CardType.Attack;

            case ConditionType.HasBuff:
                return target.HasBuff();

            case ConditionType.HasDebuff:
                return target.HasDebuff();

            case ConditionType.HasStatusEffect:
                return target.HasStatusEffect((StatusEffectTypes)conditionValue);

            case ConditionType.TargetIsWeak:
                return target.HasStatusEffect(StatusEffectTypes.Weak);

            case ConditionType.PlayerHealthBelowThreshold:
                return target.GetHealth() <= conditionValue;

            case ConditionType.EnemyHealthBelowThreshold:
                return target.GetHealth() <= conditionValue;

            default:
                return false;
        }
    }
}
