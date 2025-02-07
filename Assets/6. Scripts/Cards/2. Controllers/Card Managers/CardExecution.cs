using UnityEngine;

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
        if (cardBehavior == null || cardBehavior.cardData == null)
        {
            Debug.LogError("[CardExecution] ❌ CardBehavior or CardData is missing.");
            return;
        }

        // Get the card effect
        CardEffect effect = cardBehavior.cardData.CardEffect;
        if (effect == null)
        {
            Debug.LogError($"[CardExecution] ❌ No effect found for card {cardBehavior.cardData.CardName}.");
            return;
        }

        // Apply the effect
        Debug.Log($"[CardExecution] 🎯 Applying {effect.name} to {target}.");
        effect.ApplyEffect(target, cardBehavior.cardData.EffectValue);
    }
}

