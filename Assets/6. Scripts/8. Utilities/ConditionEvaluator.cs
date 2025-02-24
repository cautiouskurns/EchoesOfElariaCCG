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
                // ✅ Safely check if LastCardPlayed is null before accessing its properties
                if (CardManager.Instance.LastCardPlayed == null)
                {
                    Debug.LogWarning("[ConditionEvaluator] ⚠️ No LastCardPlayed detected. Condition failed.");
                    return false;
                }
                return CardManager.Instance.LastCardPlayed.CardType == CardType.Attack;

            case ConditionType.HasBuff:
                return target != null && target.HasBuff();

            case ConditionType.HasDebuff:
                return target != null && target.HasDebuff();

            case ConditionType.HasStatusEffect:
                return target != null && target.HasStatusEffect((StatusEffectTypes)conditionValue);

            case ConditionType.TargetIsWeak:
                return target != null && target.HasStatusEffect(StatusEffectTypes.Weak);

            case ConditionType.PlayerHealthBelowThreshold:
                return target != null && target.GetHealth() <= conditionValue;

            case ConditionType.EnemyHealthBelowThreshold:
                return target != null && target.GetHealth() <= conditionValue;

            default:
                return false;
        }
    }
}
