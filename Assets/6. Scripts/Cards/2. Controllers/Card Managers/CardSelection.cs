using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Handles selecting and deselecting a card.
/// </summary>
public class CardSelection : MonoBehaviour, IPointerClickHandler
{
    private static CardBehavior currentlySelectedCard;
    private bool isSelected = false;
    public static event Action<CardBehavior> OnCardSelected;
    public static event Action<CardBehavior> OnCardDeselected;

    private CardBehavior cardBehavior;
    private CardDisplay cardDisplay; // ✅ Reference to CardDisplay

    public bool IsSelected => isSelected;

    private void Awake()
    {
        cardBehavior = GetComponent<CardBehavior>();
        cardDisplay = GetComponent<CardDisplay>(); // ✅ Get reference to display

        DeselectCard(); // Ensure it's deselected initially
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardBehavior == null)
        {
            Debug.LogError("[CardSelection] ❌ No CardBehavior found on this object!");
            return;
        }

        if (currentlySelectedCard == cardBehavior)
        {
            DeselectCard();
        }
        else
        {
            SelectCard();
        }
    }

    private void SelectCard()
    {
        if (currentlySelectedCard != null)
        {
            currentlySelectedCard.GetComponent<CardSelection>().DeselectCard();
        }

        isSelected = true;
        currentlySelectedCard = cardBehavior;

        cardDisplay?.SetSelectionHighlight(true); // ✅ Notify CardDisplay

        OnCardSelected?.Invoke(cardBehavior);
    }

    private void DeselectCard()
    {
        if (!isSelected) return;

        isSelected = false;
        cardDisplay?.SetSelectionHighlight(false); // ✅ Notify CardDisplay

        OnCardDeselected?.Invoke(cardBehavior);

        if (currentlySelectedCard == cardBehavior)
        {
            currentlySelectedCard = null;
        }
    }

    public static CardBehavior GetSelectedCard()
    {
        return currentlySelectedCard;
    }
}

