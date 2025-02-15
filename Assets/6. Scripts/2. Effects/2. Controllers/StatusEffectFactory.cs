using UnityEngine;
using System.Collections.Generic;

public class StatusEffectFactory : MonoBehaviour
{
    [SerializeField] private List<BaseStatusEffect> statusEffectDatabase;  // ✅ Store status effects
    private Dictionary<StatusEffectTypes, BaseStatusEffect> statusEffectLookup = new Dictionary<StatusEffectTypes, BaseStatusEffect>();

    private void Awake()
    {
        foreach (var effect in statusEffectDatabase)
        {
            if (!statusEffectLookup.ContainsKey(effect.StatusType))
                statusEffectLookup.Add(effect.StatusType, effect);
        }
    }

    public BaseStatusEffect CreateStatusEffect(StatusEffectTypes type)
    {
        if (!statusEffectLookup.TryGetValue(type, out BaseStatusEffect effectData))
        {
            Debug.LogError($"[StatusEffectFactory] ❌ No status effect found for type: {type}");
            return null;
        }

        BaseStatusEffect newEffect = Instantiate(effectData); // ✅ Clone scriptable object
        return newEffect;
    }
}
