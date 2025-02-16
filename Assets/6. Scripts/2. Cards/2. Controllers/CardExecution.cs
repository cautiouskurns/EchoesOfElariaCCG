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

        Debug.Log($"[CardExecution] 🎴 Playing {cardBehavior.CardData.CardName}");

        // ✅ Play sound before applying effect
        cardBehavior?.PlayCardSound();

        // ✅ Play attack animation before applying effect
        StartCoroutine(PerformAttackSequence(target));
    }

    /// <summary>
    /// ✅ Moves the character, plays animation, then resolves the card effects.
    /// </summary>
    private IEnumerator PerformAttackSequence(IEffectTarget target)
    {
        if (target == null)
        {
            Debug.LogError("[CardExecution] ❌ Target is null in PerformAttackSequence");
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

        // ✅ Move forward and perform attack animation
        Vector3 targetPosition = ((MonoBehaviour)target).transform.position;
        yield return StartCoroutine(animationController.PlayAttackSequence(targetPosition, cardBehavior.CardData.CardType));

        // ✅ Pass card execution to `CardManager`
        if (CardManager.Instance != null)
        {
            CardManager.Instance.PlayCard(cardBehavior.CardData, target);
        }
        else
        {
            Debug.LogError("[CardExecution] ❌ CardManager instance is missing!");
        }

        // ✅ Remove the card from the player's hand
        if (HandManager.Instance != null)
        {
            HandManager.Instance.DiscardCard(gameObject);
        }
        else
        {
            Debug.LogError("[CardExecution] ❌ HandManager instance is missing!");
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
}

