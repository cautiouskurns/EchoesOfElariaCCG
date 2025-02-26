using UnityEngine;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

public abstract class BaseCharacter : MonoBehaviour, ICharacter, IEffectTarget
{
    public string Name { get; protected set; }
    [SerializeField] private Sprite portrait;
    public CharacterStats Stats { get; private set; }
    public CharacterCombat Combat { get; private set; }
    public CharacterEffects Effects { get; private set; }
    public CharacterSelection Selection { get; private set; }
    public CharacterTurnManager TurnManager { get; private set; }
    public CharacterDeathHandler DeathHandler { get; private set; }

    private static BaseCharacter currentlySelectedCharacter;
    public static BaseCharacter GetSelectedCharacter() => currentlySelectedCharacter;


    protected virtual void Awake()
    {
        Stats = GetComponent<CharacterStats>();
        Combat = GetComponent<CharacterCombat>();
        Effects = GetComponent<CharacterEffects>();
        Selection = GetComponent<CharacterSelection>();    
        TurnManager = GetComponent<CharacterTurnManager>();
        DeathHandler = GetComponent<CharacterDeathHandler>();
    }


        /// ✅ **Unified Initialization for Both Players and Enemies**
    public virtual void InitializeFromClass(ICharacterClass characterClass)
    {
        if (characterClass == null)
        {
            Debug.LogError("[BaseCharacter] ❌ CharacterClass is null!");
            return;
        }

        Name = characterClass.ClassName;
        portrait = characterClass.ClassIcon;

        // ✅ Initialize all stats from CharacterClass (whether Player or Enemy)
        Stats.Initialize(
            characterClass.BaseHealth,
            characterClass.BaseEnergy,
            characterClass.Strength,
            characterClass.Dexterity,
            characterClass.Intelligence,
            characterClass.Luck
        );

        Debug.Log($"[BaseCharacter] ✅ {Name} initialized (HP: {Stats.MaxHealth}, STR: {Stats.Strength}, EN: {Stats.MaxActionPoints})");
    }

    // ✅ Retrieves Stats dynamically from CharacterStats component
    public virtual int GetHealth() => Stats.CurrentHealth;
    public virtual int GetMaxHealth() => Stats.MaxHealth;
    public virtual int GetActionPoints() => Stats.CurrentActionPoints;
    public virtual int GetMaxActionPoints() => Stats.MaxActionPoints;
    public virtual int GetStrength() => Stats.Strength;
    public virtual int GetDexterity() => Stats.Dexterity;
    public virtual int GetIntelligence() => Stats.Intelligence;
    public virtual int GetLuck() => Stats.Luck;


    public virtual void Select()
    {
        if (currentlySelectedCharacter != null && currentlySelectedCharacter != this)
        {
            currentlySelectedCharacter.Deselect();
        }

        currentlySelectedCharacter = this;
        Selection?.Select();
    }

    public virtual void Deselect()
    {
        if (currentlySelectedCharacter == this)
        {
            currentlySelectedCharacter = null;
        }
        Selection?.Deselect();
    }
    
    public void ReceiveStatusEffect(IStatusEffect effect, int duration) => Effects?.ReceiveStatusEffect(effect, duration);
    public void RemoveStatusEffect(IStatusEffect effect) => Effects?.RemoveStatusEffect(effect);
        /// ✅ Checks if this character has a specific status effect
    public bool HasStatusEffect(StatusEffectTypes statusType)
    {
        return Effects.HasStatusEffect(statusType);
    }

    /// ✅ Checks if the character has an **active Buff**
    public bool HasBuff()
    {
        return Effects.HasBuff();
    }

    /// ✅ Checks if the character has an **active Debuff**
    public bool HasDebuff()
    {
        return Effects.HasDebuff();
    }

    public virtual void ProcessEndOfTurnEffects() => Effects?.ProcessEndOfTurnEffects();

    // ✅ Handles taking damage
    public virtual void TakeDamage(int damage)
    {
        // Calculate damage multiplier from all active status effects
        float damageMultiplier = 1.0f;
        
        // Check all active status effects for damage modifiers
        foreach (var activeEffect in Effects.activeEffects)
        {
            damageMultiplier *= activeEffect.EffectData.GetDamageModifier();
        }
        
        // Apply the damage multiplier
        int modifiedDamage = Mathf.RoundToInt(damage * damageMultiplier);
        
        // Apply actual damage
        Stats.ModifyHealth(-modifiedDamage);
        
        if (damageMultiplier != 1.0f)
        {
            Debug.Log($"[BaseCharacter] {Name} took {modifiedDamage} damage (base: {damage}, multiplier: {damageMultiplier:F2}). HP: {Stats.CurrentHealth}/{Stats.MaxHealth}");
        }
        else
        {
            Debug.Log($"[BaseCharacter] {Name} took {damage} damage. HP: {Stats.CurrentHealth}/{Stats.MaxHealth}");
        }

        if (Stats.CurrentHealth <= 0 && DeathHandler != null)
        {
            DeathHandler.Die();
        }
    }

    public virtual void Heal(int amount) => Stats.ModifyHealth(amount);
    public virtual void UseActionPoints(int amount) => Stats.UseActionPoints(amount);

    public void ModifyStrength(int amount) => Stats.ModifyStrength(amount);
    public void ModifyDexterity(int amount) => Stats.ModifyDexterity(amount);
    public void ModifyIntelligence(int amount) => Stats.ModifyIntelligence(amount);
    public void ModifyLuck(int amount) => Stats.ModifyLuck(amount);

    public void GainEnergy(int amount) => Stats.ModifyEnergy(amount);
    public void GainBlock(int amount) => Stats.ModifyBlock(amount);

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



