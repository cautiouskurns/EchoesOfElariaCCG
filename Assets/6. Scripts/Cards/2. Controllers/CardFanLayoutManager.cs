using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class CardFanLayoutManager : MonoBehaviour
{
    [Header("Fan Settings")]
    [SerializeField] private float curveRadius = 400f;     // Reduced from 1200f
    [SerializeField] private float fanAngle = 8f;          // Increased from 5f
    [SerializeField] private float cardSpacing = 80f;      // New: Controls horizontal spacing
    [SerializeField] private float arcHeight = 50f;        // Reduced from 100f
    [SerializeField] private float verticalOffset = 0f;

    [Header("Hover Animation")]
    [SerializeField] private float hoverLiftHeight = 50f;
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float hoverDuration = 0.2f;
    [SerializeField] private Ease hoverEase = Ease.OutQuart;

    private Dictionary<GameObject, Vector3> cardBasePositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> cardBaseRotations = new Dictionary<GameObject, Quaternion>();

    public void ArrangeCards(List<GameObject> cards)
    {
        // Clear old positions for removed cards
        var keysToRemove = cardBasePositions.Keys
            .Where(card => !cards.Contains(card))
            .ToList();
            
        foreach (var key in keysToRemove)
        {
            cardBasePositions.Remove(key);
            cardBaseRotations.Remove(key);
        }

        int cardCount = cards.Count;
        if (cardCount == 0) return;

        // Recalculate positions for remaining cards
        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = cards[i];
            Vector3 position = CalculateCardPosition(i, cardCount, cards);
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Lerp(-fanAngle, fanAngle, (float)i / (cardCount - 1)));

            cardBasePositions[card] = position;
            cardBaseRotations[card] = rotation;

            // Animate to new position
            card.transform.DOKill(); // Kill any existing tweens
            card.transform.DOLocalMove(position, 0.3f).SetEase(Ease.OutQuad);
            card.transform.DOLocalRotate(rotation.eulerAngles, 0.3f).SetEase(Ease.OutQuad);
        }
    }

    private Vector3 CalculateCardPosition(int index, int totalCards, List<GameObject> cards)
    {
        // Calculate relative position in hand (0 to 1)
        float normalizedPosition = (float)index / (totalCards - 1);
        
        // Calculate horizontal position with tighter spacing
        float x = index * cardSpacing - ((totalCards - 1) * cardSpacing * 0.5f);
        
        // Calculate vertical arc (convex upward)
        float arcOffset = -(normalizedPosition * normalizedPosition - normalizedPosition) * arcHeight * 4f;
        float y = arcOffset + verticalOffset;
        
        // Calculate rotation (fans outward)
        float rotationAngle = Mathf.Lerp(-fanAngle, fanAngle, normalizedPosition);
        cardBaseRotations[cards[index]] = Quaternion.Euler(0, 0, rotationAngle);
        
        // Layer cards properly
        float z = -index * 0.01f;
        
        return new Vector3(x, y, z);
    }

    public void OnCardHover(GameObject card, bool isHovered)
    {
        if (!cardBasePositions.ContainsKey(card)) return;

        if (isHovered)
        {
            Vector3 hoverPosition = cardBasePositions[card] + Vector3.up * hoverLiftHeight;
            card.transform.DOLocalMove(hoverPosition, hoverDuration).SetEase(hoverEase);
            card.transform.DOScale(Vector3.one * hoverScale, hoverDuration).SetEase(hoverEase);
            card.transform.DOLocalRotate(Vector3.zero, hoverDuration).SetEase(hoverEase);
        }
        else
        {
            card.transform.DOLocalMove(cardBasePositions[card], hoverDuration).SetEase(hoverEase);
            card.transform.DOScale(Vector3.one, hoverDuration).SetEase(hoverEase);
            card.transform.DOLocalRotate(cardBaseRotations[card].eulerAngles, hoverDuration).SetEase(hoverEase);
        }
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
