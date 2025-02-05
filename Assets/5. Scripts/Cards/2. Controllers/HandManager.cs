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
        if (deckManager == null)
        {
            Debug.LogError("[HandManager] ❌ DeckManager is not assigned.");
            return;
        }

        DrawCards(maxHandSize);
    }

    public void DrawCards(int number)
    {
        int cardsToDraw = Mathf.Min(number, maxHandSize - currentHand.Count);

        for (int i = 0; i < cardsToDraw; i++)
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
                cardBehavior.UpdateCardDisplay();
                deckManager.deck.RemoveAt(0);
                currentHand.Add(cardObject);

                Debug.Log($"[HandManager] ✅ Drew card: {cardBehavior.cardData.cardName}");
            }
            else
            {
                Debug.LogError("[HandManager] ❌ CardBehavior missing on cardPrefab.");
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

