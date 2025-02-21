using UnityEngine;
using System.Collections.Generic;

public class CharacterEffects : MonoBehaviour
{
    public List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();

    public delegate void OnEffectUpdated();
    public event OnEffectUpdated EffectUpdated;  // ✅ Triggers UI update when effects change


 /// ✅ Handles Receiving Status Effects
    public void ReceiveStatusEffect(IStatusEffect effect, int duration)
    {
        BaseStatusEffect statusEffect = effect as BaseStatusEffect;
        if (statusEffect == null) return;

        Debug.Log($"[BaseCharacter] Applying status effect: {statusEffect.EffectName} ({statusEffect.StatusType}) for {duration} turns.");

        ActiveStatusEffect existingEffect = activeEffects.Find(e => e.EffectData == statusEffect);
        if (existingEffect != null)
        {
            existingEffect.Duration = Mathf.Max(existingEffect.Duration, duration);
        }
        else
        {
            activeEffects.Add(new ActiveStatusEffect(statusEffect, duration));
        }

        if (statusEffect.EffectSound != null)
        {
            AudioSource.PlayClipAtPoint(statusEffect.EffectSound, transform.position);
        }

        UpdateStatusUI();
    }


    /// ✅ Handles Removing Status Effects
    public void RemoveStatusEffect(IStatusEffect effect)
    {
        BaseStatusEffect statusEffect = effect as BaseStatusEffect;
        if (statusEffect == null) return;

        activeEffects.RemoveAll(e => e.EffectData == statusEffect);
        EffectUpdated?.Invoke();
    }


    /// ✅ Tick Down Status Effects at End of Turn
    public void ProcessEndOfTurnEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].ReduceDuration();

            if (activeEffects[i].Duration <= 0)
            {
                Debug.Log($"[BaseCharacter] ❌ Lost status effect: {activeEffects[i].EffectData.EffectName}");
                activeEffects.RemoveAt(i);
            }
        }

        UpdateStatusUI();  // ✅ Make sure the UI updates after modifying status effects
    }

    public void UpdateStatusUI()
    {
        StatusEffectUI statusUI = GetComponentInChildren<StatusEffectUI>();
        
        if (statusUI == null)
        {
            return;
        }

        statusUI.UpdateStatusEffects(activeEffects);
    }
}
