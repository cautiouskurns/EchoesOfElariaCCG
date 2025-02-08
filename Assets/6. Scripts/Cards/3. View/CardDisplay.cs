using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image artworkImage;
    [SerializeField] private GameObject selectionHighlight; // ✅ Moved here

    private void Awake()
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false); // Ensure it's off initially
        }
    }

    public void UpdateCardVisual(CardData cardData)
    {
        if (cardData != null)
        {
            cardNameText.text = cardData.CardName;
            costText.text = cardData.Cost.ToString();
            artworkImage.sprite = cardData.CardArt;
        }
        else
        {
            Debug.LogError("[CardDisplay] ❌ CardData is null.");
        }
    }

    public void SetSelectionHighlight(bool isActive)
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(isActive);
        }
    }
}

