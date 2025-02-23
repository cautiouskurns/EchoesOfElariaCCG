using UnityEngine;
using System.Collections.Generic;

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance { get; private set; }

    [SerializeField] private StatusEffectFactory statusEffectFactory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (statusEffectFactory == null)
            statusEffectFactory = FindFirstObjectByType<StatusEffectFactory>();
    }

    public void ApplyStatusEffects(BaseCard card, IEffectTarget target)
    {
        if (card == null || target == null)
        {
            Debug.LogError("[StatusEffectManager] ❌ Card or target is null!");
            return;
        }

        foreach (StatusEffectData statusEffect in card.StatusEffects)
        {
            ApplySingleStatusEffect(statusEffect, target);
        }
    }

    private void ApplySingleStatusEffect(StatusEffectData statusEffectData, IEffectTarget target)
    {
        BaseStatusEffect statusEffect = statusEffectFactory.CreateStatusEffect(statusEffectData.statusType);
        if (statusEffect != null)
        {
            statusEffect.ApplyStatus(target, statusEffectData.duration);
            Debug.Log($"[StatusEffectManager] ✅ Applied {statusEffectData.statusType} ({statusEffectData.duration} turns) to {target}");
        }
        else
        {
            Debug.LogError($"[StatusEffectManager] ❌ No status effect found for {statusEffectData.statusType}");
        }
    }
}



