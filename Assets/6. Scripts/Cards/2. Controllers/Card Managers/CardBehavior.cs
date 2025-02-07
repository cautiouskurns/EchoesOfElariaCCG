using UnityEngine;

/// <summary>
/// Core card behavior that delegates responsibilities to other components.
/// </summary>
public class CardBehavior : MonoBehaviour
{
    [SerializeField] public CardData cardData; // ScriptableObject data
    private CardDisplay cardDisplay;
    private CardSelection cardSelection;
    private CardExecution cardExecution;

    public CardData CardData => cardData; // Read-only access to card data

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        cardSelection = GetComponent<CardSelection>();
        cardExecution = GetComponent<CardExecution>();

        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        if (cardDisplay != null && cardData != null)
        {
            cardDisplay.UpdateCardVisual(cardData);
        }
    }

    public void PlayCard(IEffectTarget target)
    {
        if (cardExecution != null)
        {
            cardExecution.PlayCard(target);
        }
    }
}



