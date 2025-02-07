using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    [SerializeField] private Transform handArea;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private int maxHandSize = 5;  // Changed default to 5 cards

    private List<GameObject> currentHand = new List<GameObject>();

    private void Start()
    {
        if (!ValidateSetup()) return;
        
        // Ensure hand area has proper layout
        if (handArea.GetComponent<HorizontalLayoutGroup>() == null)
        {
            HorizontalLayoutGroup layout = handArea.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }
        
        DrawCards(maxHandSize);
    }

    private bool ValidateSetup()
    {
        if (handArea == null)
        {
            Debug.LogError("[HandManager] ❌ Hand Area not assigned!");
            return false;
        }
        if (cardPrefab == null)
        {
            Debug.LogError("[HandManager] ❌ Card Prefab not assigned!");
            return false;
        }
        if (deckManager == null)
        {
            Debug.LogError("[HandManager] ❌ DeckManager not assigned!");
            return false;
        }
        if (deckManager.deck == null || deckManager.deck.Count == 0)
        {
            Debug.LogError("[HandManager] ❌ Deck is empty or null!");
            return false;
        }
        Debug.Log("[HandManager] ✅ Setup validated successfully");
        return true;
    }

    public void DrawCards(int number)
    {
        int desiredCards = Mathf.Min(number, maxHandSize - currentHand.Count);
        Debug.Log($"[HandManager] Drawing {desiredCards} cards. Current hand: {currentHand.Count}");

        // Reshuffle if needed before drawing
        if (deckManager.deck.Count < desiredCards)
        {
            deckManager.ReshuffleDeck();
        }

        for (int i = 0; i < desiredCards && deckManager.deck.Count > 0; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, handArea);
            CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();
            
            if (cardBehavior != null)
            {
                cardBehavior.cardData = deckManager.deck[0];
                cardBehavior.UpdateCardDisplay();
                deckManager.deck.RemoveAt(0);
                currentHand.Add(cardObject);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(handArea as RectTransform);
    }

    public void DiscardCard(GameObject card)
    {
        if (currentHand.Contains(card))
        {
            CardBehavior cardBehavior = card.GetComponent<CardBehavior>();
            if (cardBehavior != null && cardBehavior.cardData != null)
            {
                deckManager.discardPile.Add(cardBehavior.cardData);
            }
            currentHand.Remove(card);
            Destroy(card);
            Debug.Log("[HandManager] 🗑️ Card discarded to discard pile");
        }
    }

    public void ClearHand()
    {
        // Create a temporary list to store cards to be discarded
        List<CardData> cardsToDiscard = new List<CardData>();
        
        // First, collect all card data
        foreach (GameObject card in currentHand)
        {
            if (card != null)
            {
                CardBehavior cardBehavior = card.GetComponent<CardBehavior>();
                if (cardBehavior != null && cardBehavior.cardData != null)
                {
                    cardsToDiscard.Add(cardBehavior.cardData);
                }
            }
        }

        // Then destroy the game objects
        foreach (GameObject card in currentHand)
        {
            if (card != null)
            {
                Destroy(card);
            }
        }

        // Add collected cards to discard pile
        foreach (CardData cardData in cardsToDiscard)
        {
            deckManager.discardPile.Add(cardData);
        }

        currentHand.Clear();
        Debug.Log($"[HandManager] 🧹 Hand cleared, added {cardsToDiscard.Count} cards to discard pile");
    }

    public void RefreshHand()
    {
        ClearHand();
        deckManager.ReshuffleDeck();
        DrawCards(maxHandSize);
        Debug.Log($"[HandManager] 🔄 Hand refreshed to {currentHand.Count}/{maxHandSize} cards");
    }

}

