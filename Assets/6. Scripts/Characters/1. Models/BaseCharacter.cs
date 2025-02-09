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

    public List<StatusEffects> statusEffects = new List<StatusEffects>();

    public void GainBlock(int amount)
    {
        block += amount;
        Debug.Log($"{Name} gained {amount} Block.");
    }

    public void ModifyStrength(int amount)
    {
        strength += amount;
        Debug.Log($"{Name} gained {amount} Strength.");
    }

    public void ApplyWeak(int turns)
    {
        statusEffects.Add(new StatusEffects(StatusType.Weak, turns));
        Debug.Log($"{Name} is weakened for {turns} turns.");
    }

    public void ApplyVulnerable(int turns)
    {
        statusEffects.Add(new StatusEffects(StatusType.Vulnerable, turns));
        Debug.Log($"{Name} is vulnerable for {turns} turns.");
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


    // ✅ Apply correct effect type
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

            case EffectType.Block:
                GainBlock(value);
                Debug.Log($"{Name} gained {value} Block.");
                break;

            case EffectType.Strength:
                ModifyStrength(value);
                Debug.Log($"{Name} gained {value} Strength.");
                break;

            case EffectType.Weak:
                ApplyWeak(value);
                Debug.Log($"{Name} is weakened for {value} turns.");
                break;

            case EffectType.Vulnerable:
                ApplyVulnerable(value);
                Debug.Log($"{Name} is vulnerable for {value} turns.");
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

            default:
                Debug.LogWarning($"[BaseCharacter] Unknown effect type: {type}");
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
}

