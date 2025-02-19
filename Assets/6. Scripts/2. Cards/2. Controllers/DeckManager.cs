using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<BaseCard> allCards;  
    [SerializeField] private int deckSize = 20;  

    [Header("Card Piles (Visible in Inspector)")]
    [SerializeField] private List<BaseCard> drawPile = new List<BaseCard>();  
    [SerializeField] private List<BaseCard> discardPile = new List<BaseCard>();
    [SerializeField] private List<BaseCard> exhaustPile = new List<BaseCard>();

    public IReadOnlyList<BaseCard> DrawPile => drawPile;  
    public IReadOnlyList<BaseCard> DiscardPile => discardPile;
    public IReadOnlyList<BaseCard> ExhaustPile => exhaustPile;

    private int currentClassIndex = 0;  // Track which class's deck is active
    private Dictionary<int, List<BaseCard>> classDeckMap = new Dictionary<int, List<BaseCard>>();

    private void Awake()
    {
        if (allCards == null || allCards.Count == 0)
        {
            Debug.LogError("[DeckManager] ❌ No cards assigned to allCards!");
            return;
        }

        InitializeDeck();
    }

    private void Start()
    {
        InitializeClassDecks();
    }

    private void InitializeDeck()
    {
        Debug.Log($"[DeckManager] Initializing deck. Target deck size: {deckSize}");

        drawPile.Clear();
        discardPile.Clear();
        exhaustPile.Clear();

        if (allCards.Count < deckSize)
        {
            Debug.LogWarning("[DeckManager] ❌ Not enough cards in allCards list! Adjusting deck size.");
            deckSize = allCards.Count;
        }

        // ✅ Use the first `deckSize` cards from `allCards`
        drawPile.AddRange(allCards.GetRange(0, deckSize));

        ShuffleDeck(); // ✅ Ensures randomness at game start
        Debug.Log($"[DeckManager] ✅ Draw pile initialized with {drawPile.Count} cards.");
    }

    private void InitializeClassDecks()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[DeckManager] GameManager not found!");
            return;
        }

        // Initialize decks for each selected class
        for (int i = 0; i < GameManager.Instance.selectedClasses.Length; i++)
        {
            CharacterClass characterClass = GameManager.Instance.selectedClasses[i];
            if (characterClass != null && characterClass.startingDeck != null)
            {
                classDeckMap[i] = new List<BaseCard>(characterClass.startingDeck);
                Debug.Log($"[DeckManager] Initialized deck for {characterClass.className} with {characterClass.startingDeck.Count} cards");
            }
        }

        // Initialize with first class's deck
        SwitchToClassDeck(0);
    }

    public void SwitchToClassDeck(int classIndex)
    {
        if (!classDeckMap.ContainsKey(classIndex))
        {
            Debug.LogError($"[DeckManager] No deck found for class index {classIndex}!");
            return;
        }

        currentClassIndex = classIndex;
        drawPile.Clear();
        discardPile.Clear();
        drawPile.AddRange(classDeckMap[classIndex]);
        ShuffleDeck();
        
        Debug.Log($"[DeckManager] Switched to deck for class {classIndex}");
    }

    public void ShuffleDeck()
    {
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (drawPile[i], drawPile[j]) = (drawPile[j], drawPile[i]);
        }
    }

    public BaseCard DrawCard()
    {
        if (drawPile.Count == 0)
        {
            if (discardPile.Count > 0)
            {
                ReshuffleDiscardIntoDeck();
            }
            else
            {
                Debug.Log("[DeckManager] ❌ No cards left in either pile!");
                return null;
            }
        }

        BaseCard drawnCard = drawPile[0];
        drawPile.RemoveAt(0);
        Debug.Log($"[DeckManager] 🃏 Drew '{drawnCard.CardName}'. Draw pile: {drawPile.Count}, Discard pile: {discardPile.Count}");
        return drawnCard;
    }

    public void ReshuffleDiscardIntoDeck()
    {
        if (discardPile.Count == 0)
        {
            Debug.Log("[DeckManager] ❌ No cards in discard pile to reshuffle.");
            return;
        }

        drawPile.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
        Debug.Log($"[DeckManager] 🔄 Deck reshuffled. New deck size: {drawPile.Count}");
    }

    public void AddToDiscardPile(BaseCard card)
    {
        if (card != null)
        {
            discardPile.Add(card);
            Debug.Log($"[DeckManager] 🗑️ Added {card.CardName} to discard pile. Discard pile size: {discardPile.Count}");
        }
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



