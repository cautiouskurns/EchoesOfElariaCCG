using UnityEngine;
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

    /// <summary>
    /// ✅ Executes the effects of a card.
    /// </summary>
    public void PlayCard(BaseCard card, IEffectTarget target)
    {
        if (card == null || target == null)
        {
            Debug.LogError("[CardManager] ❌ Invalid card or target.");
            return;
        }

        Debug.Log($"[CardManager] 🎴 Executing {card.CardName} on {target}");

        // ✅ Play Sound
        if (card.SoundEffect != null)
        {
            AudioManager.Instance?.PlaySound(card.SoundEffect);
        }

        // ✅ Apply Main Effects
        foreach (EffectType effectType in card.GetEffects())  // ✅ Now using GetEffects()
        {
            effectManager?.ApplyEffect(effectType, target, card);
        }

        // ✅ Apply Status Effects
        foreach (StatusEffectTypes statusType in card.GetStatusEffects())  // ✅ Now using GetStatusEffects()
        {
            statusEffectManager?.ApplyStatusEffect(statusType, target);
        }


        // ✅ Spawn VFX if available
        if (card.VFXPrefab != null)
        {
            InstantiateEffect(card.VFXPrefab, target);
        }
    }

    /// ✅ Method to instantiate VFX
    private void InstantiateEffect(GameObject vfxPrefab, IEffectTarget target)
    {
        BaseCharacter targetCharacter = target as BaseCharacter;
        if (targetCharacter == null)
        {
            Debug.LogError("[CardManager] ❌ Target is not a valid character!");
            return;
        }

        Vector3 spawnPosition = targetCharacter.transform.position + new Vector3(0, 1, 0); // Offset for visibility
        GameObject effectInstance = Instantiate(vfxPrefab, spawnPosition, Quaternion.identity);
        
        // ✅ Optional: Destroy the effect after a delay
        Destroy(effectInstance, 2f);
    }
}


