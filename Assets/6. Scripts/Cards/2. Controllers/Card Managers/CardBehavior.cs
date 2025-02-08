using UnityEngine;
using UnityEngine.UI;
using Cards;

/// <summary>
/// Core card behavior that delegates responsibilities to other components.
/// </summary>
public class CardBehavior : MonoBehaviour
{
    [SerializeField] private CardData cardData; // ScriptableObject data
    [SerializeField] private Image cardBackground;  // Assign in inspector
    public CardData CardData => cardData; // Read-only access to card data

    private void Awake()
    {
        // Automatically find card background if not assigned
        if (cardBackground == null)
        {
            cardBackground = GetComponent<Image>();
            if (cardBackground == null)
            {
                cardBackground = GetComponentInChildren<Image>();
            }
        }
    }

    public void Initialize(CardData newCardData)
    {
        cardData = newCardData;
        UpdateCardColor();
        CardDisplay display = GetComponent<CardDisplay>();
        if (display != null)
        {
            display.UpdateCardVisual(cardData);
        }
    }

    private void UpdateCardColor()
    {
        if (cardBackground == null || cardData == null) 
        {
            Debug.LogWarning("[CardBehavior] Missing cardBackground or cardData!");
            return;
        }

        Color cardColor = GetColorForType(cardData.CardType);
        cardBackground.color = cardColor;
        Debug.Log($"[CardBehavior] Updated card color for type: {cardData.CardType}");
    }

    private Color GetColorForType(CardType type)
    {
        switch (type)
        {
            case CardType.Attack:
                return new Color(1f, 0.6f, 0.6f); // Light red
            case CardType.Spell:
                return new Color(0.6f, 0.6f, 1f); // Light blue
            case CardType.Support:
                return new Color(0.6f, 1f, 0.6f); // Light green
            default:
                return Color.white;
        }
    }
}


