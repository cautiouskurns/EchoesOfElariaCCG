using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private EffectFactory effectFactory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ApplyEffect(EffectType effectType, IEffectTarget target)
    {
        BaseEffect effect = effectFactory.CreateEffect(effectType);
        if (effect != null)
        {
            effect.ApplyEffect(target, effect.BaseValue);
        }
        else
        {
            Debug.LogError($"[EffectManager] ❌ No effect found for {effectType}");
        }
    }
}
