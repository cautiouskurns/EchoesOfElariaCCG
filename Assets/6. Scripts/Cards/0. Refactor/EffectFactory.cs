using UnityEngine;
using System.Collections.Generic;

public class EffectFactory : MonoBehaviour
{
    [SerializeField] private List<BaseEffect> effectDatabase;  // ✅ Store predefined effects
    private Dictionary<EffectType, BaseEffect> effectLookup = new Dictionary<EffectType, BaseEffect>();

    private void Awake()
    {
        foreach (var effect in effectDatabase)
        {
            if (!effectLookup.ContainsKey(effect.EffectType))
                effectLookup.Add(effect.EffectType, effect);
        }
    }

    public BaseEffect CreateEffect(EffectType type)
    {
        if (!effectLookup.TryGetValue(type, out BaseEffect effectData))
        {
            Debug.LogError($"[EffectFactory] ❌ No effect found for type: {type}");
            return null;
        }

        BaseEffect newEffect = Instantiate(effectData); // ✅ Clone scriptable object
        return newEffect;
    }
}

