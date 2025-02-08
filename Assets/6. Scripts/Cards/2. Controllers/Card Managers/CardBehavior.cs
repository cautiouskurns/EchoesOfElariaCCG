using UnityEngine;

/// <summary>
/// Core card behavior that delegates responsibilities to other components.
/// </summary>
public class CardBehavior : MonoBehaviour
{
    [SerializeField] private CardData cardData; // ScriptableObject data
    public CardData CardData => cardData; // Read-only access to card data

    public void Initialize(CardData newCardData)
    {
        cardData = newCardData;
        CardDisplay display = GetComponent<CardDisplay>();
        if (display != null)
        {
            display.UpdateCardVisual(cardData);
        }
    }
}


