using UnityEngine;
using System.Collections.Generic;

public class StatusEffectFactory : MonoBehaviour
{
    [SerializeField] private List<BaseStatusEffect> statusEffectDatabase;  // Store status effects

    public BaseStatusEffect CreateStatusEffect(StatusType type)
    {
        BaseStatusEffect effectData = statusEffectDatabase.Find(e => e.StatusType == type);
        if (effectData == null)
        {
            Debug.LogError($"[StatusEffectFactory] No status effect found for type: {type}");
            return null;
        }

        return Instantiate(effectData);  // ✅ Create a new instance of the status effect
    }
}
