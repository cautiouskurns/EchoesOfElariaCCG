using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public Image artworkImage;

    private CardBehavior card;

    private void Awake()
    {
        card = GetComponent<CardBehavior>();
        UpdateCardVisual();
    }

    public void UpdateCardVisual()
    {
        if (card.cardData != null)
        {
            cardNameText.text = card.cardData.cardName;
            costText.text = card.cardData.cost.ToString();
            artworkImage.sprite = card.cardData.cardArt;
        }
    }
}
