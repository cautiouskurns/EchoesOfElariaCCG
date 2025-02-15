using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private EffectFactory effectFactory; // ✅ Uses factory to create effects

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (effectFactory == null)
        {
            effectFactory = FindFirstObjectByType<EffectFactory>();
        }

        if (effectFactory == null)
        {
            Debug.LogError("[EffectManager] ❌ Missing EffectFactory!");
        }
    }

    /// <summary>
    /// ✅ Fetches effect from `EffectFactory` and applies it to the target.
    /// </summary>
    public void ApplyEffect(EffectType effectType, IEffectTarget target)
    {
        if (target == null)
        {
            Debug.LogError("[EffectManager] ❌ Target is null!");
            return;
        }

        BaseEffect effect = effectFactory.CreateEffect(effectType);
        if (effect != null)
        {
            effect.ApplyEffect(target, effect.BaseValue);
            Debug.Log($"[EffectManager] ✅ Applied {effectType} effect to {target}");
        }
        else
        {
            Debug.LogError($"[EffectManager] ❌ No effect found for {effectType}");
        }
    }
}
