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

        int finalValue = GetFinalEffectValue(sourceCharacter);
        
        // ✅ Play sound before applying effect
        if (cardBehavior != null)
        {
            cardBehavior.PlayCardSound();
        }

        // ✅ Play attack animation before applying effect
        StartCoroutine(PerformAttackSequence(target));
    }


    /// <summary>
    /// ✅ Moves the character, plays animation, then applies effect.
    /// </summary>
    private IEnumerator PerformAttackSequence(IEffectTarget target)
    {
        BaseCharacter sourceCharacter = BaseCharacter.GetSelectedCharacter();
        if (sourceCharacter == null)
        {
            Debug.LogError("[CardExecution] ❌ No character selected to perform attack.");
            yield break;
        }

        // ✅ Get animation controller from source character
        CharacterAnimationController animationController = sourceCharacter.GetComponentInChildren<CharacterAnimationController>();

        if (animationController == null)
        {
            Debug.LogError($"[CardExecution] ❌ CharacterAnimationController is missing on {sourceCharacter.Name}!");
            yield break;
        }

        // ✅ Move forward and perform attack
        Vector3 targetPosition = ((MonoBehaviour)target).transform.position;
        yield return StartCoroutine(animationController.PlayAttackSequence(targetPosition));

        // ✅ Apply effect after animation completes
        int finalValue = GetFinalEffectValue(sourceCharacter);
        ApplyCardEffect(target, finalValue);

        // ✅ Remove card from hand and destroy it
        HandManager handManager = FindAnyObjectByType<HandManager>();
        if (handManager != null)
        {
            handManager.DiscardCard(gameObject);
            Debug.Log("[CardExecution] Card removed from hand and discarded");
        }
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
