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
        Debug.LogError($"[HandManager] ❌ Card not in hand: {cardObject.name}");
        return;
    }

    CardBehavior cardBehavior = cardObject.GetComponent<CardBehavior>();
    if (cardBehavior == null || cardBehavior.CardData == null)
    {
        Debug.LogError("[HandManager] ❌ Invalid CardBehavior or BaseCard!");
        return;
    }

    BaseCard card = cardBehavior.CardData;
    Debug.Log($"[HandManager] Playing {card.CardName}");

    // ✅ Call `CardManager` to execute the card
    CardManager.Instance?.PlayCard(card, target);

    // ✅ Move card to discard pile AFTER playing
    deckManager.AddToDiscardPile(card);
    RemoveCardFromHand(card, cardObject);
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
            Debug.Log($"[HandManager] 🗑️ Removed {card.CardName} from hand.");
        }
        else
        {
            Debug.LogWarning($"[HandManager] ⚠️ Tried to remove {card.CardName}, but it's not in hand.");
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

    public void ClearHand()
    {
        foreach (var card in cardObjects)
        {
            Destroy(card);
        }

        deckManager.discardPile.AddRange(currentHand);
        currentHand.Clear();
        cardObjects.Clear();

        Debug.Log($"[HandManager] 🧹 Hand cleared, all cards moved to discard pile.");
    }

    public void RefreshHand()
    {
        ClearHand();
        deckManager.ReshuffleDeck();
        DrawCards(maxHandSize);
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
}




