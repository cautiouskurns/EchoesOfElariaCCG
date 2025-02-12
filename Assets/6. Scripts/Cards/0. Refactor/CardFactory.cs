using UnityEngine;
using System.Collections.Generic;

public class CardFactory : MonoBehaviour
{
    [SerializeField] private List<BaseCard> cardDatabase;  // ✅ Store all card data

    public BaseCard CreateCard(string cardName)
    {
        BaseCard cardData = cardDatabase.Find(c => c.CardName == cardName);
        if (cardData == null)
        {
            Debug.LogError($"[CardFactory] No card found with name: {cardName}");
            return null;
        }

        return Instantiate(cardData); // ✅ Return an instance of the card
    }
}
