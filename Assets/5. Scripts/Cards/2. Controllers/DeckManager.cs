using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] public List<CardData> allCards; // All available cards
    public List<CardData> deck;                      // Active deck

    private void Start()
    {
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        deck = new List<CardData>(allCards);
        for (int i = 0; i < deck.Count; i++)
        {
            CardData temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        Debug.Log("[DeckManager] ✅ Deck shuffled.");
    }
}


