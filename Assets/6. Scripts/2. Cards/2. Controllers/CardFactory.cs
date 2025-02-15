using UnityEngine;
using System.Collections.Generic;

public class CardFactory : MonoBehaviour
{
    [SerializeField] private List<BaseCard> cardDatabase;  // ✅ Store all card data
    private Dictionary<string, BaseCard> cardLookup = new Dictionary<string, BaseCard>();

    private void Awake()
    {
        foreach (var card in cardDatabase)
        {
            if (!cardLookup.ContainsKey(card.CardName))
                cardLookup.Add(card.CardName, card);
        }
    }

    public BaseCard CreateCard(string cardName)
    {
        if (!cardLookup.TryGetValue(cardName, out BaseCard cardData))
        {
            Debug.LogError($"[CardFactory] ❌ No card found with name: {cardName}");
            return null;
        }

        return Instantiate(cardData); // ✅ Create a new instance of the card
    }
}

