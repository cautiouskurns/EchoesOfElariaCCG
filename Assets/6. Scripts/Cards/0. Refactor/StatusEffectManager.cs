using UnityEngine;
using System.Collections.Generic;

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance { get; private set; }

    [SerializeField] private StatusEffectFactory statusEffectFactory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ApplyStatusEffect(StatusType statusType, IEffectTarget target)
    {
        BaseStatusEffect statusEffect = statusEffectFactory.CreateStatusEffect(statusType);
        if (statusEffect != null)
        {
            statusEffect.ApplyStatus(target, statusEffect.MaxDuration);
        }
        else
        {
            Debug.LogError($"[StatusEffectManager] ❌ No status effect found for {statusType}");
        }
    }
}

