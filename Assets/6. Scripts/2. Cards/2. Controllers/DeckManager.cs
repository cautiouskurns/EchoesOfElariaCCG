using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<BaseCard> allCards; // ✅ Changed from `CardData` to `BaseCard`
    [SerializeField] private int deckSize = 20;
    
    public List<BaseCard> deck { get; private set; } = new List<BaseCard>();  // ✅ Now using `BaseCard`
    public List<BaseCard> discardPile { get; private set; } = new List<BaseCard>(); 
    public List<BaseCard> exhaustPile = new List<BaseCard>(); 

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

        for (int i = 0; i < deckSize; i++)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            deck.Add(allCards[randomIndex]);  // ✅ Uses `BaseCard`
        }

        ShuffleDeck();
        Debug.Log($"[DeckManager] ✅ Deck initialized with {deck.Count} cards");
    }

    public void ShuffleDeck()
    {
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

    public void ExhaustCard(BaseCard card) // ✅ Changed parameter from `CardData` to `BaseCard`
    {
        if (card != null)
        {
            exhaustPile.Add(card);
            Debug.Log($"[DeckManager] 🚫 Card '{card.CardName}' has been exhausted.");
        }
    }
}


