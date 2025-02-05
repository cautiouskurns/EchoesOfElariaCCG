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
        if (cardData != null && cardData.cardEffect != null)
        {
            Debug.Log($"[CardBehavior] 🎯 Playing {cardData.cardName} on {target.Name}");

            // Apply the card's effect using the ScriptableObject
            cardData.cardEffect.ApplyEffect(target, cardData.effectValue);
        }
        else
        {
            Debug.LogWarning($"[CardBehavior] ⚠️ No CardEffect assigned for {cardData?.cardName}");
        }

        Destroy(gameObject); // Remove the card from the hand after playing
    }
}



