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

    protected virtual void Awake()
    {
        Stats = GetComponent<CharacterStats>();
        Combat = GetComponent<CharacterCombat>();
    }

    public virtual void TakeDamage(int damage) => Stats.ModifyHealth(-damage);
    public virtual void Heal(int amount) => Stats.ModifyHealth(amount);
    public virtual void UseActionPoints(int amount) => Stats.UseActionPoints(amount);

    public List<StatusEffect> statusEffects = new List<StatusEffect>();
    public List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();

    public delegate void OnEffectUpdated();
    public event OnEffectUpdated EffectUpdated;  // ✅ Triggers UI update when effects change

    public void ModifyStrength(int amount)
    {
        strength += amount;
        Debug.Log($"{Name} gained {amount} Strength.");
    }

    public void GainEnergy(int amount)
    {
        energy += amount;
        Debug.Log($"{Name} gained {amount} Energy.");
    }

    public void DrawCards(int amount)
    {
        //DeckManager.DrawCards(amount);
    }

    public void ExhaustCard()
    {
        //HandManager.ExhaustRandomCard();
    }

    public void ApplyPowerEffect(int value)
    {
        // Powers are persistent effects. You can implement them here.
        //PowerManager.Instance.ApplyPowerEffect(this, value);
    }

    // ✅ Apply Status Effects
    public void ApplyStatusEffect(StatusEffectData effect, int duration)
    {
        Debug.Log($"[BaseCharacter] Applying status effect: {effect.effectName} ({effect.statusType}) for {duration} turns.");

        ActiveStatusEffect existingEffect = activeEffects.Find(e => e.effectData == effect);
        if (existingEffect != null)
        {
            existingEffect.duration = Mathf.Max(existingEffect.duration, duration);
        }
        else
        {
            activeEffects.Add(new ActiveStatusEffect(effect, duration));
        }

        if (effect.effectSound != null)
        {
            AudioSource.PlayClipAtPoint(effect.effectSound, transform.position);
        }

        DebugStatusEffects();
        
        // ✅ Directly call UpdateStatusUI
        UpdateStatusUI(); 
    }


    public void RemoveEffect(StatusEffectData effect)
    {
        activeEffects.RemoveAll(e => e.effectData == effect);
        EffectUpdated?.Invoke();
    }

    // ✅ Reduce Status Effect Durations (Call at end of turn)
    public void ProcessEndOfTurnEffects()
    {
        foreach (var effect in activeEffects)
        {
            effect.duration--;
        }
        activeEffects.RemoveAll(e => e.duration <= 0);
        EffectUpdated?.Invoke();
    }
    public void ApplyEffect(int value, EffectType type)
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


    public virtual void Select()
    {
        // Deselect current character if one exists and it's not this one
        if (currentlySelectedCharacter != null && currentlySelectedCharacter != this)
        {
            currentlySelectedCharacter.Deselect();
        }

        // Only set as selected if not already selected
        if (!IsSelected)
        {
            currentlySelectedCharacter = this;
            isSelected = true;
            Debug.Log($"[BaseCharacter] Selected character: {Name} (Class: {Stats.CharacterClass})");
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

    public static BaseCharacter GetSelectedCharacter() => currentlySelectedCharacter;

    public void DebugStatusEffects()
    {
        Debug.Log($"[BaseCharacter] {Name} Status Effects:");

        if (statusEffects.Count == 0)
        {
            Debug.Log(" - No active status effects.");
            return;
        }

        foreach (var effect in statusEffects)
        {
            Debug.Log($" - {effect.Type} (Duration: {effect.Duration} turns)");
        }
    }

    public void EndTurn()
    {
        Debug.Log($"[BaseCharacter] {Name} ending turn...");

        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            statusEffects[i].Duration--;

            if (statusEffects[i].Duration <= 0)
            {
                Debug.Log($"{Name} lost {statusEffects[i].Type} effect.");
                statusEffects.RemoveAt(i);
            }
        }

        // ✅ Log updated effects
        DebugStatusEffects();
    }

    public void UpdateStatusUI()
    {
        StatusEffectUI statusUI = GetComponentInChildren<StatusEffectUI>();
        if (statusUI != null)
        {
            List<StatusEffect> statusList = new List<StatusEffect>();

            foreach (var effect in activeEffects)
            {
                statusList.Add(new StatusEffect(effect.effectData.statusType, effect.duration, effect.effectData));
            }

            statusUI.UpdateStatusEffects(statusList);  // ✅ Now passing a compatible list
        }
    }

}


// ✅ Helper class to store effect data + duration
[System.Serializable]
public class ActiveStatusEffect
{
    public StatusEffectData effectData;
    public int duration;

    public ActiveStatusEffect(StatusEffectData data, int dur)
    {
        effectData = data;
        duration = dur;
    }
}