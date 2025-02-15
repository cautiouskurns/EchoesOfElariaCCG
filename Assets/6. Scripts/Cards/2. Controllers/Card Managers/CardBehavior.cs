using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cards;

public class CardBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BaseCard cardData;  // ✅ Now uses `BaseCard`
    [SerializeField] private Image cardBackground;
    
    public BaseCard CardData => cardData;  // ✅ Exposes `BaseCard` to other scripts
    private HandManager handManager;

    private void Awake()
    {
        if (cardBackground == null)
        {
            cardBackground = GetComponent<Image>();
        }

        // Ensure UI interaction is enabled
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        foreach (var image in GetComponentsInChildren<Image>())
        {
            image.raycastTarget = true;
        }

        // Locate HandManager in scene
        FindHandManager();
    }

    private void FindHandManager()
    {
        handManager = FindFirstObjectByType<HandManager>();
        
        if (handManager == null)
        {
            Debug.LogError($"[CardBehavior] ❌ Could not find HandManager in scene! Make sure HandManager component exists.");
        }
        else
        {
            Debug.Log($"[CardBehavior] ✅ Found HandManager on {handManager.gameObject.name}");
        }
    }

    /// <summary>
    /// ✅ Assigns new card data and updates UI accordingly.
    /// </summary>
    public void Initialize(BaseCard newCardData)
    {
        cardData = newCardData;
        UpdateCardColor();
        
        CardDisplay display = GetComponent<CardDisplay>();
        if (display != null)
        {
            display.UpdateCardVisual(cardData);
        }
    }

    /// <summary>
    /// ✅ Updates card color based on card type.
    /// </summary>
    private void UpdateCardColor()
    {
        if (cardBackground == null || cardData == null) return;
        cardBackground.color = GetColorForType(cardData.CardType);
    }

    /// <summary>
    /// ✅ Returns appropriate color based on card type.
    /// </summary>
    private Color GetColorForType(CardType type)
    {
        return type switch
        {
            CardType.Attack => new Color(1f, 0.6f, 0.6f),  // Light red
            CardType.Spell => new Color(0.6f, 0.6f, 1f),   // Light blue
            CardType.Support => new Color(0.6f, 1f, 0.6f), // Light green
            _ => Color.white
        };
    }

    /// <summary>
    /// ✅ Plays sound when a card is played.
    /// </summary>
    public void PlayCardSound()
    {
        if (AudioManager.Instance != null && cardData != null && cardData.SoundEffect != null)
        {
            AudioManager.Instance.PlaySound(cardData.SoundEffect);  
            Debug.Log($"[CardBehavior] 🎵 Playing sound: {cardData.SoundEffect.name}");
        }
        else
        {
            Debug.LogWarning("[CardBehavior] ❌ Cannot play sound - Missing AudioManager, CardData, or SoundEffect!");
        }
    }

    /// <summary>
    /// ✅ Handles when the player hovers over the card.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (handManager == null)
        {
            FindHandManager();
            if (handManager == null) return;
        }
        handManager.OnCardHover(gameObject, true);
    }

    /// <summary>
    /// ✅ Handles when the player stops hovering over the card.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (handManager == null) return;
        handManager.OnCardHover(gameObject, false);
    }
}

