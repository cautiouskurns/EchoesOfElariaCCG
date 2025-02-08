using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("Required Text Elements")]
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI costText;

    [Header("Optional Text Elements")]
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Visual Elements")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private Image cardFrame;
    [SerializeField] private GameObject selectionHighlight;

    private void Awake()
    {
        ValidateComponents();
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false);
        }
    }

    public void UpdateCardVisual(CardData cardData)
    {
        if (cardData == null)
        {
            Debug.LogError("[CardDisplay] ❌ CardData is null!");
            return;
        }

        // Update required elements
        if (cardNameText) cardNameText.text = cardData.CardName;
        if (costText) costText.text = cardData.Cost.ToString();
        
        // Update optional description if available
        if (descriptionText != null)
        {
            descriptionText.text = cardData.CardDescription;
        }
        
        // Update artwork
        if (artworkImage && cardData.CardArt)
        {
            artworkImage.sprite = cardData.CardArt;
            artworkImage.preserveAspect = true;
        }

        Debug.Log($"[CardDisplay] Updated card visual for: {cardData.CardName}");
    }

    private void ValidateComponents()
    {
        if (!cardNameText) Debug.LogError("[CardDisplay] Missing card name text component!");
        if (!costText) Debug.LogError("[CardDisplay] Missing cost text component!");
        if (!artworkImage) Debug.LogError("[CardDisplay] Missing artwork image component!");
        
        // Description text is optional, just log a warning
        if (!descriptionText) Debug.LogWarning("[CardDisplay] Description text component not assigned - description will not be displayed.");
    }

    public void SetSelectionHighlight(bool isActive)
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(isActive);
        }
    }

    // Validate components in editor
    private void OnValidate()
    {
        if (!cardNameText || !costText || !descriptionText || !artworkImage)
        {
            Debug.LogWarning("[CardDisplay] Some UI elements are not assigned in inspector!");
        }
    }
}

