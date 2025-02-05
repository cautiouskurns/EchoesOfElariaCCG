using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private Transform handArea;    // Reference to the UI Panel where cards are displayed
    [SerializeField] private GameObject cardPrefab; // The Card Prefab
    [SerializeField] private DeckManager deckManager; // Reference to the DeckManager

    private void Start()
    {
        if (deckManager == null)
        {
            Debug.LogError("[HandManager] ❌ DeckManager is not assigned.");
            return;
        }

        DrawCards(5);
    }

    public void DrawCards(int number)
    {
        for (int i = 0; i < number; i++)
        {
            if (deckManager.deck.Count == 0)
            {
                Debug.LogWarning("[HandManager] 🃏 Deck is empty. No cards to draw.");
                return;
            }

            GameObject cardObject = Instantiate(cardPrefab, handArea);
            CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();

            if (cardBehavior != null)
            {
                cardBehavior.cardData = deckManager.deck[0];
                cardBehavior.UpdateCardDisplay();  // Ensure the card UI updates
                deckManager.deck.RemoveAt(0);

                Debug.Log($"[HandManager] ✅ Drew card: {cardBehavior.cardData.cardName}");
            }
            else
            {
                Debug.LogError("[HandManager] ❌ CardBehavior missing on cardPrefab.");
            }
        }
    }
}
