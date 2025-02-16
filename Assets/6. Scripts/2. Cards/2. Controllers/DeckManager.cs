using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<BaseCard> allCards;
    [SerializeField] private int deckSize = 20;

    [Header("Card Piles (Visible in Inspector)")]
    [SerializeField] public List<BaseCard> drawPile = new List<BaseCard>();  
    [SerializeField] public List<BaseCard> discardPile = new List<BaseCard>();
    [SerializeField] public List<BaseCard> exhaustPile = new List<BaseCard>();

    public IReadOnlyList<BaseCard> DrawPile => drawPile;  // Read-only access
    public IReadOnlyList<BaseCard> DiscardPile => discardPile;
    public IReadOnlyList<BaseCard> ExhaustPile => exhaustPile;

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
        drawPile.Clear();
        discardPile.Clear();
        exhaustPile.Clear();

        for (int i = 0; i < deckSize; i++)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            drawPile.Add(allCards[randomIndex]);
        }

        ShuffleDeck();
        Debug.Log($"[DeckManager] ✅ Deck initialized with {drawPile.Count} cards");
    }

    public void ShuffleDeck()
    {
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (drawPile[i], drawPile[j]) = (drawPile[j], drawPile[i]);
        }
    }

    public void ReshuffleDeck()
    {
        Debug.Log($"[DeckManager] Reshuffling. Draw Pile: {drawPile.Count}, Discard: {discardPile.Count}");
        if (discardPile.Count == 0) return;

        drawPile.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
        Debug.Log($"[DeckManager] 🔄 Deck reshuffled. New size: {drawPile.Count}");
    }

    public BaseCard DrawCard()
    {
        if (drawPile.Count == 0)
        {
            ReshuffleDeck();
            if (drawPile.Count == 0)
            {
                Debug.Log("[DeckManager] ❌ No cards left to draw!");
                return null;
            }
        }

        BaseCard drawnCard = drawPile[0];
        drawPile.RemoveAt(0);
        return drawnCard;
    }

    public void AddToDiscardPile(BaseCard card)
    {
        discardPile.Add(card);
        Debug.Log($"[DeckManager] 🗑️ {card.CardName} moved to discard pile.");
    }

    public void ExhaustCard(BaseCard card)
    {
        if (card != null)
        {
            exhaustPile.Add(card);
            Debug.Log($"[DeckManager] 🚫 {card.CardName} has been exhausted.");
        }
    }

    public int GetDrawPileCount() => drawPile.Count;
    public int GetDiscardPileCount() => discardPile.Count;
}



