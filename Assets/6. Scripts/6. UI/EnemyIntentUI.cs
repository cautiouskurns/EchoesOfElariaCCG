using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyIntentUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image cardIcon;
    [SerializeField] private TextMeshProUGUI intentText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.2f;
    [SerializeField] private float floatHeight = 0.5f;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        HideInstant();
    }

    public void ShowIntent(BaseCard card)
    {
        if (card == null)
        {
            Debug.LogError("[EnemyIntentUI] Attempted to show intent with null card!");
            return;
        }

        // Ensure UI is visible
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;  // Set immediately visible
        canvasGroup.blocksRaycasts = true;

        // Update UI elements
        if (cardIcon != null) cardIcon.sprite = card.CardArt;
        if (intentText != null) intentText.text = $"Will use {card.CardName}";

        Debug.Log($"[EnemyIntentUI] Showing intent: {card.CardName}");
    }

    public void HideIntent()
    {
        transform.DOKill();
        canvasGroup.DOKill();

        Sequence hideSequence = DOTween.Sequence();
        hideSequence.Join(canvasGroup.DOFade(0, fadeOutDuration))
                   .Join(transform.DOLocalMoveY(0, fadeOutDuration))
                   .SetEase(Ease.InQuad)
                   .OnComplete(() => {
                       canvasGroup.blocksRaycasts = false;
                       gameObject.SetActive(false);
                   });
    }

    private void HideInstant()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
