using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Handles selecting and deselecting a card.
/// </summary>
public class CardSelection : MonoBehaviour, IPointerClickHandler
{
    private static CardSelection currentlySelectedCard;
    private bool isSelected = false;

    [SerializeField] private GameObject selectionHighlight; // Highlight effect
    public static event Action<CardSelection> OnCardSelected;
    public static event Action<CardSelection> OnCardDeselected;

    public bool IsSelected => isSelected;

    private void Awake()
    {
        // Auto-assign selection highlight if null
        if (selectionHighlight == null)
        {
            selectionHighlight = transform.Find("SelectionHighlight")?.gameObject;
        }

        DeselectCard(); // Ensure it's deselected initially
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentlySelectedCard != null && currentlySelectedCard != this)
        {
            currentlySelectedCard.DeselectCard();
        }

        if (!isSelected)
        {
            SelectCard();
        }
        else
        {
            DeselectCard();
        }
    }

    private void SelectCard()
    {
        isSelected = true;
        currentlySelectedCard = this;
        selectionHighlight?.SetActive(true);
        OnCardSelected?.Invoke(this);
    }

    private void DeselectCard()
    {
        isSelected = false;
        selectionHighlight?.SetActive(false);
        OnCardDeselected?.Invoke(this);

        if (currentlySelectedCard == this)
        {
            currentlySelectedCard = null;
        }
    }

    public static CardSelection GetSelectedCard()
    {
        return currentlySelectedCard;
    }
}
