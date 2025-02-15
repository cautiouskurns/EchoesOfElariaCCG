using UnityEngine;
using System.Collections.Generic;

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance { get; private set; }

    [SerializeField] private StatusEffectFactory statusEffectFactory; // ✅ Uses factory to create status effects

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (statusEffectFactory == null)
        {
            statusEffectFactory = FindFirstObjectByType<StatusEffectFactory>();
        }

        if (statusEffectFactory == null)
        {
            Debug.LogError("[StatusEffectManager] ❌ Missing StatusEffectFactory!");
        }
    }

    /// <summary>
    /// ✅ Fetches status effect from `StatusEffectFactory` and applies it to the target.
    /// </summary>
    public void ApplyStatusEffect(StatusEffectTypes statusType, IEffectTarget target)
    {
        if (target == null)
        {
            Debug.LogError("[StatusEffectManager] ❌ Target is null!");
            return;
        }

        BaseStatusEffect statusEffect = statusEffectFactory.CreateStatusEffect(statusType);
        if (statusEffect != null)
        {
            statusEffect.ApplyStatus(target, statusEffect.MaxDuration);
            Debug.Log($"[StatusEffectManager] ✅ Applied {statusType} status effect to {target}");
        }
        else
        {
            Debug.LogError($"[StatusEffectManager] ❌ No status effect found for {statusType}");
        }
    }
}


