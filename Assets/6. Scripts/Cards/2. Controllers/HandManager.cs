using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private Transform handArea;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private int maxHandSize = 3;  // Set max hand size in Inspector

    private List<GameObject> currentHand = new List<GameObject>();

    private void Start()
    {
        if (!ValidateSetup()) return;
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
        int cardsToDraw = Mathf.Min(number, maxHandSize - currentHand.Count);
        Debug.Log($"[HandManager] Drawing {cardsToDraw} cards. Current hand size: {currentHand.Count}");

        for (int i = 0; i < cardsToDraw; i++)
        {
            if (deckManager.deck.Count == 0) break;

            // Create card in hand area
            GameObject cardObject = Instantiate(cardPrefab, handArea);
            RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
            
            if (rectTransform != null)
            {
                // Ensure proper positioning
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }

            CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();

            if (cardBehavior != null)
            {
                cardBehavior.cardData = deckManager.deck[0];
                cardBehavior.UpdateCardDisplay();
                deckManager.deck.RemoveAt(0);
                currentHand.Add(cardObject);
                Debug.Log($"[HandManager] ✅ Added {cardBehavior.cardData.CardName} to hand");
            }
        }
    }

    public void DiscardCard(GameObject card)
    {
        if (currentHand.Contains(card))
        {
            currentHand.Remove(card);
            Destroy(card);
            Debug.Log("[HandManager] 🗑️ Card discarded.");
        }
    }
}

