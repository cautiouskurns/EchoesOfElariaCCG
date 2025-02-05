using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<CardData> allCards;
    public List<CardData> deck;

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
    }
}

