using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private EffectFactory effectFactory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (effectFactory == null)
            effectFactory = FindFirstObjectByType<EffectFactory>();
    }

    public void ApplyEffects(BaseCard card, IEffectTarget target)
    {
        if (card == null || target == null)
        {
            Debug.LogError("[EffectManager] ❌ Card or target is null!");
            return;
        }

        foreach (EffectData effect in card.Effects)
        {
            ApplySingleEffect(effect, target);
        }
    }

    public void ApplySingleEffect(EffectData effectData, IEffectTarget target)
    {
        BaseEffect effect = effectFactory.CreateEffect(effectData.effectType);
        if (effect != null)
        {
            effect.ApplyEffect(target, effectData.value);
            Debug.Log($"[EffectManager] ✅ Applied {effectData.effectType} ({effectData.value}) to {target}");
        }
        else
        {
            Debug.LogError($"[EffectManager] ❌ No effect found for {effectData.effectType}");
        }
    }
}
