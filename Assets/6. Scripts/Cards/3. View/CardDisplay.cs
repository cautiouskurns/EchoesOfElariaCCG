using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image artworkImage;

    public void UpdateCardVisual(CardData cardData)
    {
        if (cardData != null)
        {
            cardNameText.text = cardData.CardName;
            costText.text = cardData.Cost.ToString();
            artworkImage.sprite = cardData.CardArt;

            Debug.Log($"[CardDisplay] ✅ Display updated for {cardData.CardName}");
        }
        else
        {
            Debug.LogError("[CardDisplay] ❌ CardData is null.");
        }
    }
}

