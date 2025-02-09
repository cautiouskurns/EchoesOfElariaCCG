using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cards;

public class CardBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardData cardData;
    [SerializeField] private Image cardBackground;
    public CardData CardData => cardData;
    private HandManager handManager;

    private void Awake()
    {
        // Setup components
        if (cardBackground == null)
        {
            cardBackground = GetComponent<Image>();
        }

        // Setup UI interaction
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

        // Find HandManager in hierarchy
        FindHandManager();
    }

    private void FindHandManager()
    {
        // First try to find HandManager in scene
        handManager = FindFirstObjectByType<HandManager>();
        
        if (handManager == null)
        {
            Debug.LogError($"[CardBehavior] Could not find HandManager in scene! Make sure HandManager component exists.");
        }
        else
        {
            Debug.Log($"[CardBehavior] Found HandManager on {handManager.gameObject.name}");
        }
    }

    public void Initialize(CardData newCardData)
    {
        cardData = newCardData;
        UpdateCardColor();
        CardDisplay display = GetComponent<CardDisplay>();
        if (display != null)
        {
            display.UpdateCardVisual(cardData);
        }
    }

    private void UpdateCardColor()
    {
        if (cardBackground == null || cardData == null) return;

        cardBackground.color = GetColorForType(cardData.CardType);
    }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (handManager == null)
        {
            FindHandManager();
            if (handManager == null) return;
        }
        // Debug.Log($"[CardBehavior] Hover ENTER on {gameObject.name}");
        handManager.OnCardHover(gameObject, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (handManager == null) return;
        Debug.Log($"[CardBehavior] Hover EXIT on {gameObject.name}");
        handManager.OnCardHover(gameObject, false);
    }
}
