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

        // Get card cost
        int cost = cardBehavior.cardData.Cost;
        
        // Check if enough AP is available
        if (!APManager.Instance.SpendAP(cost))
        {
            Debug.LogWarning($"[CardExecution] ❌ Not enough AP to play {cardBehavior.cardData.CardName}");
            return;
        }

        // Apply the card effect
        CardEffect effect = cardBehavior.cardData.CardEffect;
        if (effect == null)
        {
            Debug.LogError($"[CardExecution] ❌ No effect found for card {cardBehavior.cardData.CardName}.");
            return;
        }

        Debug.Log($"[CardExecution] 🎯 Playing {cardBehavior.cardData.CardName} on {target}.");
        effect.ApplyEffect(target, cardBehavior.cardData.EffectValue);

        // Remove the card from hand
        Destroy(gameObject);
    }
}

