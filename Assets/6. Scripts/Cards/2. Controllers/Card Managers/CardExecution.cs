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
        StartCoroutine(PerformAttackSequence(target, finalValue));
    }


    /// <summary>
    /// ✅ Moves the character, plays animation, then applies effect.
    /// </summary>
    private IEnumerator PerformAttackSequence(IEffectTarget target, int finalValue)
    {
        if (target == null)
        {
            Debug.LogError("Target is null in PerformAttackSequence");
            yield break;
        }

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
        try
        {
            ApplyCardEffect(target, finalValue);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in PerformAttackSequence: {e.Message}");
        }

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
        if (target == null)
        {
            Debug.LogError("[CardExecution] ❌ Target is NULL in ApplyCardEffect");
            return;
        }

        if (cardBehavior == null)
        {
            Debug.LogError("[CardExecution] ❌ cardBehavior is NULL in ApplyCardEffect");
            return;
        }

        if (cardBehavior.CardData == null)
        {
            Debug.LogError($"[CardExecution] ❌ CardData is NULL in {cardBehavior.name}");
            return;
        }

        if (cardBehavior.CardData.CardEffect == null)
        {
            Debug.LogError($"[CardExecution] ❌ CardEffect is NULL in {cardBehavior.CardData.CardName}");
            return;
        }

        CardEffect effect = cardBehavior.CardData.CardEffect;
        if (effect == null)
        {
            Debug.LogError($"[CardExecution] ❌ effect is NULL in {cardBehavior.CardData.CardName}");
            return;
        }

        if (effect.effectData == null)
        {
            Debug.LogError($"[CardExecution] ❌ effectData is NULL in {effect.name}");
            return;
        }


        BaseCharacter targetCharacter = target as BaseCharacter;
        if (targetCharacter == null)
        {
            Debug.LogError("[CardExecution] ❌ Target is not a valid BaseCharacter");
            return;
        }

        Debug.Log($"✅ [CardExecution] {BaseCharacter.GetSelectedCharacter()?.Name} played {cardBehavior.CardData.CardName}");

        // ✅ Apply the main card effect (damage, block, etc.)
        try
        {
            Debug.Log($"🔹 Applying {effect.GetType().Name} effect to {targetCharacter.Name}");
            targetCharacter.ReceiveEffect(finalValue, effect.EffectType);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error applying primary effect: {e.Message}\nStack trace: {e.StackTrace}");
        }

        // ✅ Apply any status effects attached to the card
        if (cardBehavior.CardData.StatusEffects != null && cardBehavior.CardData.StatusEffects.Count > 0)
        {
            foreach (var statusEffect in cardBehavior.CardData.StatusEffects)
            {
                if (statusEffect == null)
                {
                    Debug.LogError($"❌ Null status effect detected in {cardBehavior.CardData.CardName}");
                    continue;
                }

                if (statusEffect.maxDuration <= 0)
                {
                    Debug.LogError($"❌ Invalid duration for {statusEffect.name}: {statusEffect.maxDuration}");
                    continue;
                }

                Debug.Log($"🔹 Applying status effect {statusEffect.effectName} to {targetCharacter.Name}");
                targetCharacter.ApplyStatusEffect(statusEffect, statusEffect.maxDuration);
            }
        }
        else
        {
            Debug.Log($"🔹 No status effects found for {cardBehavior.CardData.CardName}");
        }
    }

}
