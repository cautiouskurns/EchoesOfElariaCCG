using UnityEngine;
using System.Collections;

public class CardExecution : MonoBehaviour
{
    private CardBehavior cardBehavior;

    private void Awake()
    {
        cardBehavior = GetComponent<CardBehavior>();
        if (cardBehavior == null)
        {
            Debug.LogError("[CardExecution] ❌ CardBehavior component is missing!");
        }
    }

    public void PlayCard(IEffectTarget target)
    {
        if (!ValidatePlayConditions()) return;

        BaseCharacter sourceCharacter = BaseCharacter.GetSelectedCharacter();
        if (sourceCharacter == null)
        {
            Debug.LogWarning("[CardExecution] ❌ No character selected to play card!");
            return;
        }

        StartCoroutine(PerformAttackSequence(sourceCharacter, target));
    }

    /// <summary>
    /// ✅ Moves the character, plays animation, then applies effect.
    /// </summary>
    private IEnumerator PerformAttackSequence(BaseCharacter sourceCharacter, IEffectTarget target)
    {
        // ✅ Now searches for the animation controller on both the parent and children
        CharacterAnimationController animationController = sourceCharacter.GetComponentInChildren<CharacterAnimationController>();

        if (animationController == null)
        {
            Debug.LogError($"[CardExecution] ❌ CharacterAnimationController is missing on {sourceCharacter.Name}!");
            yield break;
        }

        // Move forward & perform attack sequence
        Vector3 targetPosition = ((MonoBehaviour)target).transform.position;
        yield return StartCoroutine(animationController.PlayAttackSequence(targetPosition));

        // Wait for the full sequence to complete before applying damage
        yield return new WaitForSeconds(0.5f);

        int finalValue = GetFinalEffectValue(sourceCharacter);
        ApplyCardEffect(target, finalValue);
    }


    private bool ValidatePlayConditions()
    {
        if (TurnManager.Instance.CurrentTurn != TurnManager.TurnState.PlayerTurn)
        {
            Debug.LogWarning("[CardExecution] ❌ Cannot play cards during enemy turn!");
            return false;
        }

        if (cardBehavior == null || cardBehavior.CardData == null)
        {
            Debug.LogError("[CardExecution] ❌ CardBehavior or CardData is missing.");
            return false;
        }

        if (!APManager.Instance.SpendAP(cardBehavior.CardData.Cost))
        {
            Debug.LogWarning($"[CardExecution] ❌ Not enough AP to play {cardBehavior.CardData.CardName}");
            return false;
        }

        return true;
    }

    private int GetFinalEffectValue(BaseCharacter sourceCharacter)
    {
        int baseValue = cardBehavior.CardData.EffectValue;
        float multiplier = (sourceCharacter.Stats.CharacterClass == cardBehavior.CardData.PreferredClass)
            ? cardBehavior.CardData.ClassBonus
            : 1.0f;

        int finalValue = Mathf.RoundToInt(baseValue * multiplier);
        Debug.Log($"[CardExecution] Damage calculation: {baseValue} × {multiplier} = {finalValue}");
        return finalValue;
    }

    private void ApplyCardEffect(IEffectTarget target, int finalValue)
    {
        CardEffect effect = cardBehavior.CardData.CardEffect;
        if (effect == null)
        {
            Debug.LogError($"[CardExecution] ❌ No effect found for card {cardBehavior.CardData.CardName}.");
            return;
        }

        Debug.Log($"[CardExecution] 🎯 {BaseCharacter.GetSelectedCharacter().Name} played {cardBehavior.CardData.CardName} for {finalValue} damage");
        effect.ApplyEffect(target, finalValue);
    }
}
