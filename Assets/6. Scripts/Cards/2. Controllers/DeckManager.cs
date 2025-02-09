using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<CardData> allCards;
    [SerializeField] private int deckSize = 20;  // Set fixed deck size
    public List<CardData> deck { get; private set; } = new List<CardData>();
    public List<CardData> discardPile { get; private set; } = new List<CardData>();
    public List<CardData> exhaustPile = new List<CardData>(); // ✅ New Exhaust Pile


    private void Awake()
    {
        if (allCards == null || allCards.Count == 0)
        {
            Debug.LogError("[DeckManager] ❌ No cards assigned to allCards!");
            return;
        }
        
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        deck.Clear();
        discardPile.Clear();

        // Fill deck up to deckSize, repeating cards if necessary
        for (int i = 0; i < deckSize; i++)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            deck.Add(allCards[randomIndex]);
        }

        ShuffleDeck();
        Debug.Log($"[DeckManager] ✅ Deck initialized with {deck.Count} cards");
    }

    public void ShuffleDeck()
    {
        // Fisher-Yates shuffle
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    public void ReshuffleDeck()
    {
        Debug.Log($"[DeckManager] Reshuffling. Deck: {deck.Count}, Discard: {discardPile.Count}");
        deck.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
        Debug.Log($"[DeckManager] 🔄 Deck reshuffled. New size: {deck.Count}");
    }

    public void ExhaustCard(CardData card)
    {
        if (card != null)
        {
            exhaustPile.Add(card);
            Debug.Log($"[DeckManager] 🚫 Card '{card.CardName}' has been exhausted.");
        }
    }
}


