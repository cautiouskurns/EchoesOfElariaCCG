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
    public void ApplyEffect(EffectType effectType, IEffectTarget target, BaseCard sourceCard)
    {
        if (target == null || sourceCard == null)
        {
            Debug.LogError("[EffectManager] ❌ Target or source card is null!");
            return;
        }

        BaseEffect effect = effectFactory.CreateEffect(effectType);
        if (effect != null)
        {
            int effectValue = sourceCard.GetEffectValue(effectType); // ✅ Get damage/heal amount from card
            Debug.Log($"[EffectManager] Applying {effectType} with value {effectValue} to {target}");
            effect.ApplyEffect(target, effectValue);
        }
        else
        {
            Debug.LogError($"[EffectManager] ❌ No effect found for {effectType}");
        }
    }

}
