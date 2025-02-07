using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Handles card selection, interaction, and execution.
/// </summary>
public class CardBehavior : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public CardData cardData; // Card's data (ScriptableObject)
    private CardDisplay cardDisplay;

    private bool isSelected = false;
    private static CardBehavior currentlySelectedCard; // Singleton for selected card

    [SerializeField] private GameObject selectionHighlight; // Optional highlight effect

    // ✅ Event system for UI updates
    public static event Action<CardBehavior> OnCardSelected;
    public static event Action<CardBehavior> OnCardDeselected;

    /// <summary>
    /// Get the currently selected card.
    /// </summary>
    public static CardBehavior CurrentlySelected => currentlySelectedCard;

    /// <summary>
    /// Checks if this card is currently selected.
    /// </summary>
    public bool IsSelected => isSelected;

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();

        // ✅ Auto-assign the selectionHighlight if it's not set in the Inspector
        if (selectionHighlight == null)
        {
            selectionHighlight = transform.Find("SelectionHighlight")?.gameObject;
            if (selectionHighlight == null)
            {
                Debug.LogWarning("[CardBehavior] ⚠️ No selectionHighlight found.");
            }
        }

        UpdateCardDisplay();
        DeselectCard(); // Ensure default state
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[CardBehavior] Card clicked: {cardData?.CardName ?? "Unknown Card"}");

        // If another card is selected, deselect it first
        if (currentlySelectedCard != null && currentlySelectedCard != this)
        {
            currentlySelectedCard.DeselectCard();
        }

        // Toggle selection state
        if (!isSelected)
        {
            SelectCard();
        }
        else
        {
            DeselectCard();
        }
    }

    /// <summary>
    /// Selects this card and notifies the system.
    /// </summary>
    private void SelectCard()
    {
        isSelected = true;
        currentlySelectedCard = this;

        if (selectionHighlight != null)
            selectionHighlight.SetActive(true); 

        OnCardSelected?.Invoke(this); // 🔹 Notify UI handlers (e.g., glow effect)

        Debug.Log($"[CardBehavior] ✅ Selected card: {cardData.CardName}");
    }

    /// <summary>
    /// Deselects this card and resets highlight.
    /// </summary>
    private void DeselectCard()
    {
        isSelected = false;
        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);

        OnCardDeselected?.Invoke(this); // 🔹 Notify UI handlers

        if (currentlySelectedCard == this)
            currentlySelectedCard = null;
    }

    /// <summary>
    /// Plays the card on a target and removes it from the hand.
    /// </summary>
    public void PlayCard(IEffectTarget target)
    {
        if (cardData == null || cardData.CardEffect == null)
        {
            Debug.LogError("[CardBehavior] ❌ CardEffect is null. Cannot play.");
            return;
        }

        Debug.Log($"[CardBehavior] 🎯 Playing {cardData.CardName} on {target}");
        cardData.CardEffect.ApplyEffect(target, cardData.EffectValue);

        // 🔹 Remove from hand after playing
        Destroy(gameObject);
    }

    /// <summary>
    /// Updates the UI to reflect card data.
    /// </summary>
    public void UpdateCardDisplay()
    {
        if (cardDisplay != null && cardData != null)
        {
            cardDisplay.UpdateCardVisual(cardData);
        }
    }

    /// <summary>
    /// Retrieves the currently selected card.
    /// </summary>
    public static CardBehavior GetSelectedCard()
    {
        return currentlySelectedCard;
    }
}





