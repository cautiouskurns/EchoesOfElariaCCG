using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance { get; private set; }

    [SerializeField] private Transform handArea;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private int maxHandSize = 5;
    [SerializeField] private CardFanLayoutManager fanLayout;

    private List<BaseCard> currentHand = new List<BaseCard>();
    private List<GameObject> cardObjects = new List<GameObject>(); // Tracks UI instances

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (!ValidateSetup()) return;
        DrawCards(maxHandSize);
    }

    private bool ValidateSetup()
    {
        if (handArea == null || cardPrefab == null || deckManager == null)
        {
            Debug.LogError("[HandManager] ❌ Missing references!");
            return false;
        }
        return true;
    }

    public void PlayCard(GameObject cardObject, IEffectTarget target)
    {
        if (!cardObjects.Contains(cardObject))
        {
            Debug.LogError($"[HandManager] Card not in hand: {cardObject.name}");
            return;
        }

        CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();
        if (cardBehavior == null || cardBehavior.CardData == null) return;

        BaseCard card = cardBehavior.CardData;

        // Execute card effect
        CardManager.Instance?.PlayCard(card, target);

        // Move to discard pile and remove from hand
        deckManager.AddToDiscardPile(card);
        RemoveCardFromHand(card, cardObject);
        
        // Don't automatically draw new cards after playing
        Debug.Log($"[HandManager] '{card.CardName}' played and moved to discard. Hand size: {currentHand.Count}");
    }

    public void DrawCards(int number)
    {
        int desiredCards = Mathf.Min(number, maxHandSize - currentHand.Count);

        for (int i = 0; i < desiredCards; i++)
        {
            BaseCard drawnCard = deckManager.DrawCard();
            if (drawnCard == null) break; // No more cards to draw

            GameObject cardObject = Instantiate(cardPrefab, handArea);
            CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();

            if (cardBehavior != null)
            {
                cardBehavior.Initialize(drawnCard);
                currentHand.Add(drawnCard);
                cardObjects.Add(cardObject);
                Debug.Log($"[HandManager] ✅ Added card: {drawnCard.CardName}");
            }
        }

        fanLayout?.ArrangeCards(cardObjects);
    }

    private void RemoveCardFromHand(BaseCard card, GameObject cardObject)
    {
        if (currentHand.Contains(card))
        {
            currentHand.Remove(card);
            cardObjects.Remove(cardObject);
            Destroy(cardObject);
        }

        fanLayout?.ArrangeCards(cardObjects);
    }

    public void DiscardCard(GameObject cardObject)
    {
        CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();
        if (cardBehavior == null || cardBehavior.CardData == null) return;

        deckManager.AddToDiscardPile(cardBehavior.CardData);
        RemoveCardFromHand(cardBehavior.CardData, cardObject);
    }

    public void RefreshHand()
    {
        // Draw up to max hand size at start of turn
        int cardsNeeded = maxHandSize - currentHand.Count;
        if (cardsNeeded > 0)
        {
            DrawCards(cardsNeeded);
            Debug.Log($"[HandManager] Drew {cardsNeeded} cards at turn start. Hand size: {currentHand.Count}");
        }
    }

    public void ExhaustCard(GameObject cardObject)
    {
        CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();
        if (cardBehavior == null || cardBehavior.CardData == null) return;

        deckManager.ExhaustCard(cardBehavior.CardData);
        RemoveCardFromHand(cardBehavior.CardData, cardObject);

        Debug.Log($"[HandManager] 🚫 Exhausted card '{cardBehavior.CardData.CardName}'");
    }

    public void ExhaustRandomCard()
    {
        if (currentHand.Count == 0) return;

        int randomIndex = Random.Range(0, cardObjects.Count);
        ExhaustCard(cardObjects[randomIndex]);
    }

    public void OnCardHover(GameObject card, bool isHovered)
    {
        fanLayout?.OnCardHover(card, isHovered);
    }

    public void SwitchActiveClass(int classIndex)
    {
        Debug.Log($"[HandManager] Switching to class {classIndex}'s hand");
        ClearCurrentHand();
        deckManager.SwitchToClassDeck(classIndex);
        DrawCards(maxHandSize);
    }

    private void ClearCurrentHand()
    {
        foreach (var cardObj in cardObjects)
        {
            Destroy(cardObj);
        }
        currentHand.Clear();
        cardObjects.Clear();
    }
}




