using UnityEngine;
using UnityEngine.UI;
using Cards;
using UnityEngine.EventSystems;

public class CardBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardData cardData;
    [SerializeField] private Image cardBackground;
    public CardData CardData => cardData;
    private HandManager handManager;

    private void Awake()
    {
        if (cardBackground == null)
        {
            cardBackground = GetComponent<Image>();
        }
    }

    private void Start()
    {
        handManager = GetComponentInParent<HandManager>();
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
        handManager?.OnCardHover(gameObject, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        handManager?.OnCardHover(gameObject, false);
    }
}
