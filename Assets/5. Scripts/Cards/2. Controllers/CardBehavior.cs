using UnityEngine;

public class CardBehavior : MonoBehaviour
{
    public CardData cardData; // Data for the card
    private CardDisplay cardDisplay;

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
    }

    private void Start()
    {
        UpdateCardDisplay(); // Ensure card UI is set when instantiated
    }

    public void UpdateCardDisplay()
    {
        if (cardDisplay != null && cardData != null)
        {
            cardDisplay.UpdateCardVisual(cardData);
        }
        else
        {
            Debug.LogWarning("[CardBehavior] ⚠️ Missing CardDisplay or CardData.");
        }
    }

    public void PlayCard(BaseCharacter target)
    {
        Debug.Log($"[CardBehavior] 🎯 Playing {cardData.cardName} on {target.Name}");

        if (cardData.effectType == CardEffectType.Damage)
        {
            target.TakeDamage(cardData.effectValue);
        }
        else if (cardData.effectType == CardEffectType.Heal)
        {
            target.Heal(cardData.effectValue);
        }

        Destroy(gameObject); // Remove the card from the hand after playing
    }
}



