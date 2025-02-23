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

    public void ApplyEffects(BaseCard card, IEffectTarget clickedTarget)
    {
        if (card == null || clickedTarget == null)
        {
            Debug.LogError("[EffectManager] ❌ Card or target is null!");
            return;
        }

        foreach (EffectData effect in card.Effects)
        {
            // Get all relevant targets for this effect
            List<IEffectTarget> targets = ResolveTargets(effect.target, clickedTarget);
            
            foreach (var target in targets)
            {
                ApplySingleEffect(effect, target);
            }
        }
    }

    public List<IEffectTarget> ResolveTargets(EffectTarget targetType, IEffectTarget clickedTarget)
    {
        List<IEffectTarget> targets = new List<IEffectTarget>();
        
        switch (targetType)
        {
            case EffectTarget.Self:
                if (BaseCharacter.GetSelectedCharacter() != null)
                    targets.Add(BaseCharacter.GetSelectedCharacter());
                break;

            case EffectTarget.SingleEnemy:
                if (clickedTarget != null)
                    targets.Add(clickedTarget);
                break;

            case EffectTarget.SingleAlly:
                if (clickedTarget != null)
                    targets.Add(clickedTarget);
                break;

            case EffectTarget.AllEnemies:
                targets.AddRange(FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None));
                break;

            case EffectTarget.AllAllies:
                targets.AddRange(FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None));
                break;

            case EffectTarget.AllUnits:
                targets.AddRange(FindObjectsByType<BaseCharacter>(FindObjectsSortMode.None));
                break;
        }

        return targets;
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
