using UnityEngine;
using System.Collections.Generic;

public class StatusEffectFactory : MonoBehaviour
{
    [SerializeField] private List<BaseStatusEffect> statusEffectDatabase;  // ✅ Store status effects
    private Dictionary<StatusType, BaseStatusEffect> statusEffectLookup = new Dictionary<StatusType, BaseStatusEffect>();

    private void Awake()
    {
        foreach (var effect in statusEffectDatabase)
        {
            if (!statusEffectLookup.ContainsKey(effect.StatusType))
                statusEffectLookup.Add(effect.StatusType, effect);
        }
    }

    public BaseStatusEffect CreateStatusEffect(StatusType type)
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
