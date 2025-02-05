using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public Transform handArea;
    public GameObject cardPrefab;
    public List<CardData> deck = new List<CardData>();

    private void Start()
    {
        DrawCards(5);
    }

    public void DrawCards(int number)
    {
        for (int i = 0; i < number; i++)
        {
            if (deck.Count == 0) return;

            GameObject cardObject = Instantiate(cardPrefab, handArea);
            CardBehavior card = cardObject.GetComponent<CardBehavior>();
            card.cardData = deck[0];
            deck.RemoveAt(0);
        }
    }
}
