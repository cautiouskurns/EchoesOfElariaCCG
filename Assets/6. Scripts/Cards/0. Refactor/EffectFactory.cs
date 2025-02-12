using UnityEngine;
using System.Collections.Generic;

public class EffectFactory : MonoBehaviour
{
    [SerializeField] private List<BaseEffect> effectDatabase;  // Store predefined effect types

    public BaseEffect CreateEffect(EffectType type)
    {
        BaseEffect effectData = effectDatabase.Find(e => e.EffectType == type);
        if (effectData == null)
        {
            Debug.LogError($"[EffectFactory] No effect found for type: {type}");
            return null;
        }

        return Instantiate(effectData);  // ✅ Create a new instance of the effect
    }
}

