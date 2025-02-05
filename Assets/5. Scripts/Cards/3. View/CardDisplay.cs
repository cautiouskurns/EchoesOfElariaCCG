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
            cardNameText.text = cardData.cardName;
            costText.text = cardData.cost.ToString();
            artworkImage.sprite = cardData.cardArt;

            Debug.Log($"[CardDisplay] ✅ Display updated for {cardData.cardName}");
        }
        else
        {
            Debug.LogError("[CardDisplay] ❌ CardData is null.");
        }
    }
}

