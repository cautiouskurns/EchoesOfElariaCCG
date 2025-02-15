using UnityEngine;
using Cards;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [SerializeField] private EffectManager effectManager;
    [SerializeField] private StatusEffectManager statusEffectManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Resolves a card's effects before passing them to EffectManager and StatusEffectManager.
    /// </summary>
    public void ResolveCard(BaseCard card, IEffectTarget target)
    {
        if (card == null || target == null)
        {
            Debug.LogError("[CardManager] ❌ Invalid card or target.");
            return;
        }

        Debug.Log($"[CardManager] Resolving card: {card.CardName} on {target}");

        // ✅ Apply Main Effects via EffectManager
        foreach (EffectType effectType in card.EffectTypes)
        {
            effectManager.ApplyEffect(effectType, target);
        }

        // ✅ Apply Status Effects via StatusEffectManager
        foreach (StatusType statusType in card.StatusTypes)
        {
            statusEffectManager.ApplyStatusEffect(statusType, target);
        }
    }
}
