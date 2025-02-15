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
        if (card == null) return;

        // Ensure GameObject is active
        gameObject.SetActive(true);

        // Set content
        cardIcon.sprite = card.CardArt;
        intentText.text = $"Will use {card.CardName}";

        // Reset position before animating
        // transform.localPosition = Vector3.zero;

        // Animate
        transform.DOKill();
        canvasGroup.DOKill();

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = true;

        Sequence showSequence = DOTween.Sequence();
        showSequence.Join(canvasGroup.DOFade(1, fadeInDuration))
                   .Join(transform.DOLocalMoveY(floatHeight, fadeInDuration))
                   .SetEase(Ease.OutQuad);

        Debug.Log($"[EnemyIntentUI] Showing intent for {card.CardName}");
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
