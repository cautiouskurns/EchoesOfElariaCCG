using UnityEngine;
using System.Collections.Generic;

public class EffectFactory : MonoBehaviour
{
    [SerializeField] private List<BaseEffect> effectDatabase;  
    private Dictionary<EffectType, BaseEffect> effectLookup = new Dictionary<EffectType, BaseEffect>();

    private void Awake()
    {
        Debug.Log("[EffectFactory] 🔄 Initializing effect database...");

        foreach (var effect in effectDatabase)
        {
            if (!effectLookup.ContainsKey(effect.EffectType))
            {
                effectLookup.Add(effect.EffectType, effect);
                Debug.Log($"[EffectFactory] ✅ Added effect: {effect.EffectType}");
            }
            else
            {
                Debug.LogWarning($"[EffectFactory] ⚠️ Duplicate effect detected: {effect.EffectType}");
            }
        }

        Debug.Log($"[EffectFactory] 📌 Total effects loaded: {effectLookup.Count}");
    }

    public BaseEffect CreateEffect(EffectType type)
    {
        if (!effectLookup.TryGetValue(type, out BaseEffect effectData))
        {
            Debug.LogError($"[EffectFactory] ❌ No effect found for type: {type}");
            return null;
        }

        Debug.Log($"[EffectFactory] 🎯 Successfully created effect: {type}");
        return Instantiate(effectData);
    }
}

