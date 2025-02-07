using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Add this

public class CardBehavior : MonoBehaviour, IPointerClickHandler  // Implement interface
{
    public CardData cardData; // Card data (ScriptableObject)
    private CardDisplay cardDisplay;

    private bool isSelected = false;
    private static CardBehavior currentlySelectedCard; // Only one card can be selected at a time

    [SerializeField] private GameObject selectionHighlight; // Now referencing the entire GameObject

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();

        // Auto-assign the selectionHighlight if not manually set
        if (selectionHighlight == null)
        {
            selectionHighlight = transform.Find("SelectionHighlight")?.gameObject;
            if (selectionHighlight != null)
                Debug.Log("[CardBehavior] ✅ Auto-assigned selectionHighlight.");
            else
                Debug.LogWarning("[CardBehavior] ⚠️ No selectionHighlight found.");
        }

        UpdateCardDisplay();
        DeselectCard(); // Ensure it's deselected by default
    }

    // Replace OnMouseDown with this
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[CardBehavior] Card clicked: {cardData?.CardName ?? "Unknown Card"}");
        
        if (currentlySelectedCard != null && currentlySelectedCard != this)
        {
            currentlySelectedCard.DeselectCard();
        }

        isSelected = !isSelected;
        if (isSelected)
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
        currentlySelectedCard = this;
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(true); // ✅ Activate the GameObject
        }
        Debug.Log($"[CardBehavior] ✅ Selected card: {cardData.CardName}");
    }

    private void DeselectCard()
    {
        isSelected = false;
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false); // ✅ Deactivate the GameObject
        }
        else
        {
            Debug.LogWarning($"[CardBehavior] ⚠️ No selectionHighlight assigned for {cardData?.CardName ?? "Unknown Card"}.");
        }

        if (currentlySelectedCard == this)
        {
            currentlySelectedCard = null;
        }
    }

    public void PlayCard(BaseCharacter target)
    {
        Debug.Log($"[CardBehavior] 🎯 Playing {cardData.CardName} on {target.Name}");

        if (cardData.CardEffect != null)
        {
            cardData.CardEffect.ApplyEffect(target, cardData.EffectValue);
        }

        Destroy(gameObject); // Remove the card from hand after playing
    }

    public static CardBehavior GetSelectedCard() => currentlySelectedCard;

    public void UpdateCardDisplay()
    {
        if (cardDisplay != null && cardData != null)
        {
            cardDisplay.UpdateCardVisual(cardData);
        }
    }
}





