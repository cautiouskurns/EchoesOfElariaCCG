using UnityEngine;
using System.Collections.Generic;
using Cards;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [SerializeField] private EffectManager effectManager;  // ✅ Now exposed in Inspector
    [SerializeField] private StatusEffectManager statusEffectManager;  // ✅ Now exposed in Inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // If not assigned in the Inspector, try to find them automatically
        if (effectManager == null)
            effectManager = FindAnyObjectByType<EffectManager>();

        if (statusEffectManager == null)
            statusEffectManager = FindAnyObjectByType<StatusEffectManager>();

        if (effectManager == null || statusEffectManager == null)
        {
            Debug.LogError("[CardManager] ❌ Missing EffectManager or StatusEffectManager!");
        }
    }

    public void PlayCard(BaseCard card, IEffectTarget clickedTarget)
    {
        if (card == null || clickedTarget == null)
        {
            Debug.LogError("[CardManager] ❌ Invalid card or target.");
            return;
        }

        Debug.Log($"[CardManager] 🎴 Executing {card.CardName}");

        // Handle VFX for each effect's targets
        if (card.VFXPrefab != null)
        {
            foreach (EffectData effect in card.Effects)
            {
                List<IEffectTarget> targets = effectManager.ResolveTargets(effect.target, clickedTarget);
                foreach (var target in targets)
                {
                    if (target is MonoBehaviour targetMono)
                    {
                        InstantiateVFX(card.VFXPrefab, targetMono.transform.position);
                    }
                }
            }
        }

        // Play sound effect
        if (card.SoundEffect != null)
        {
            AudioManager.Instance?.PlaySound(card.SoundEffect);
        }

        // Apply effects and status effects
        effectManager?.ApplyEffects(card, clickedTarget);
        
        if (card.StatusEffects != null && card.StatusEffects.Count > 0)
        {
            statusEffectManager?.ApplyStatusEffects(card, clickedTarget);
        }
    }

    private void InstantiateVFX(GameObject vfxPrefab, Vector3 position)
    {
        Vector3 spawnPos = position + Vector3.up;
        GameObject vfx = Instantiate(vfxPrefab, spawnPos, Quaternion.identity);
        Destroy(vfx, 2f);
        Debug.Log($"[CardManager] 🎆 VFX spawned at {spawnPos}");
    }
}


