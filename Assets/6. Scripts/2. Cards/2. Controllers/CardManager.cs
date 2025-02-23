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

    public void PlayCard(BaseCard card, IEffectTarget target)
    {
        if (card == null || target == null)
        {
            Debug.LogError("[CardManager] ❌ Invalid card or target.");
            return;
        }

        Debug.Log($"[CardManager] 🎴 Executing {card.CardName} on {target}");

        // First: Spawn VFX
        if (card.VFXPrefab != null)
        {
            InstantiateEffect(card.VFXPrefab, target);
            Debug.Log($"[CardManager] 🎇 VFX spawned for {card.CardName}");
        }

        // Second: Play Sound
        if (card.SoundEffect != null)
        {
            AudioManager.Instance?.PlaySound(card.SoundEffect);
        }

        // Third: Apply Effects
        effectManager?.ApplyEffects(card, target);

        // Fourth: Apply Status Effects
        if (card.StatusEffects != null && card.StatusEffects.Count > 0)
        {
            foreach (var statusEffect in card.StatusEffects)
            {
                statusEffectManager?.ApplyStatusEffects(card, target);
            }
        }
    }

    private void InstantiateEffect(GameObject vfxPrefab, IEffectTarget target)
    {
        if (target is MonoBehaviour targetMono)
        {
            Vector3 spawnPosition = targetMono.transform.position + Vector3.up;
            GameObject effectInstance = Instantiate(vfxPrefab, spawnPosition, Quaternion.identity);
            Destroy(effectInstance, 2f);
            Debug.Log($"[CardManager] VFX instantiated at {spawnPosition}");
        }
        else
        {
            Debug.LogError("[CardManager] ❌ Target is not a valid GameObject!");
        }
    }
}


