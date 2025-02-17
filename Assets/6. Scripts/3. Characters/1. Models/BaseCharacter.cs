using UnityEngine;
using System.Collections.Generic;

public abstract class BaseCharacter : MonoBehaviour, ICharacter, IEffectTarget
{
    public string Name { get; protected set; }
    public CharacterStats Stats { get; private set; }

    [SerializeField] private int health;
    [SerializeField] private int block;
    [SerializeField] private int strength;
    [SerializeField] private int energy;
    

    public CharacterCombat Combat { get; private set; }

    private bool isSelected = false;
    public bool IsSelected => isSelected;
    private static BaseCharacter currentlySelectedCharacter;
    public int GetHealth() => health;

    protected virtual void Awake()
    {
        Stats = GetComponent<CharacterStats>();
        Combat = GetComponent<CharacterCombat>();
    }

    /// ✅ Effect Tracking
    public List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();

    public delegate void OnEffectUpdated();
    public event OnEffectUpdated EffectUpdated;  // ✅ Triggers UI update when effects change

    // ✅ Stat Modifiers
    public virtual void TakeDamage(int damage)
    {
        Stats.ModifyHealth(-damage);
        
        // Check for death after taking damage
        if (Stats.CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount) => Stats.ModifyHealth(amount);
    public virtual void UseActionPoints(int amount) => Stats.UseActionPoints(amount);
    public void ModifyStrength(int amount) => strength += amount;
    public void GainEnergy(int amount) => energy += amount;
    public void GainBlock(int amount) => block += amount;


    // ✅ Card & Power System Hooks
    public void DrawCards(int amount) => HandManager.Instance.DrawCards(amount);
    public void ExhaustCard() => HandManager.Instance.ExhaustRandomCard();
    public void ApplyPowerEffect(int value) => Debug.Log($"[BaseCharacter] {Name} applied Power Effect!");

    /// ✅ Handles Receiving Direct Effects
    public void ReceiveEffect(int value, EffectType type)
    {
        switch (type)
        {
            case EffectType.Damage:
                TakeDamage(value);
                Debug.Log($"{Name} took {value} damage.");
                break;

            case EffectType.Heal:
                Heal(value);
                Debug.Log($"{Name} healed {value} HP.");
                break;

            case EffectType.Energy:
                GainEnergy(value);
                Debug.Log($"{Name} gained {value} energy.");
                break;

            case EffectType.CardDraw:
                DrawCards(value);
                Debug.Log($"{Name} drew {value} cards.");
                break;

            case EffectType.Exhaust:
                ExhaustCard();
                Debug.Log($"{Name} exhausted a card.");
                break;

            case EffectType.Power:
                ApplyPowerEffect(value);
                Debug.Log($"{Name} activated a persistent power.");
                break;
        }
    }

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

        DebugStatusEffects();
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
        Debug.Log($"[BaseCharacter] {Name} processing end-of-turn effects...");

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
        DebugStatusEffects();
    }

    // ✅ Selection & UI Management
    public virtual void Select()
    {
        if (currentlySelectedCharacter != null && currentlySelectedCharacter != this)
        {
            currentlySelectedCharacter.Deselect();
        }

        if (!IsSelected)
        {
            currentlySelectedCharacter = this;
            isSelected = true;
            Debug.Log($"[BaseCharacter] Selected character: {Name}");
        }
    }

    public virtual void Deselect()
    {
        if (IsSelected)
        {
            currentlySelectedCharacter = null;
            isSelected = false;
            Debug.Log($"[BaseCharacter] Deselected character: {Name}");
        }
    }

    public void EndTurn()
    {
Debug.Log($"[BaseCharacter] {Name} ending turn...");
    
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].ReduceDuration();  // ✅ Reduce duration

            if (activeEffects[i].Duration <= 0)
            {
                Debug.Log($"{Name} lost {activeEffects[i].EffectData.StatusType} effect.");
                activeEffects.RemoveAt(i);
            }
        }

        // ✅ Log updated effects
        DebugStatusEffects();
}


    public static BaseCharacter GetSelectedCharacter() => currentlySelectedCharacter;

    public void DebugStatusEffects()
    {
        Debug.Log($"[BaseCharacter] {Name} Status Effects:");

        if (activeEffects.Count == 0)
        {
            Debug.Log(" - No active status effects.");
            return;
        }

        foreach (var effect in activeEffects)
        {
            Debug.Log($" - {effect.EffectData.EffectName} ({effect.Duration} turns)");
        }
    }

    public void UpdateStatusUI()
    {
        StatusEffectUI statusUI = GetComponentInChildren<StatusEffectUI>();
        
        if (statusUI == null)
        {
            Debug.LogError($"[BaseCharacter] ❌ No StatusEffectUI found for {Name}");
            return;
        }

        statusUI.UpdateStatusEffects(activeEffects);
    }

    public void SetHealth(int value)
    {
        health = Mathf.Max(value, 0); // ✅ Prevent negative values
    }

    protected virtual void Die()
    {
        Debug.Log($"[BaseCharacter] {Name} has been defeated!");
        
        // Trigger death effects/animations before destroying
        OnDeath();
        
        // Delay destruction slightly to allow for effects
        Destroy(gameObject, 1f);
    }

    protected virtual void OnDeath()
    {
        // Override in derived classes for specific death behaviors
    }
}

// ✅ Helper class to store effect data + duration
[System.Serializable]
public class ActiveStatusEffect
{
    public BaseStatusEffect EffectData { get; private set; }
    public int Duration { get; set; }

    public ActiveStatusEffect(BaseStatusEffect effect, int duration)
    {
        EffectData = effect;
        Duration = duration;
    }

    public void ReduceDuration()
    {
        Duration--;
    }
}



