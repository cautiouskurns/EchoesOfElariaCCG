using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxActionPoints { get; private set; }
    public int CurrentActionPoints { get; private set; }
    
    public int Strength { get; private set; }
    public int Dexterity { get; private set; }
    public int Intelligence { get; private set; }
    public int Luck { get; private set; }
    public int Energy { get; private set; }
    public int Block { get; private set; }

    // ✅ Events for Health & Action Points 
    public event Action<int> OnHealthChanged;
    public event Action<int> OnActionPointsChanged;

    public void Initialize(int maxHealth, int actionPoints, int strength, int dexterity, int intelligence, int luck)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        MaxActionPoints = actionPoints;
        CurrentActionPoints = actionPoints;

        Strength = strength;
        Dexterity = dexterity;
        Intelligence = intelligence;
        Luck = luck;
    }

    // public void ModifyHealth(int amount) => CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
    public void UseActionPoints(int amount) => CurrentActionPoints = Mathf.Max(0, CurrentActionPoints - amount);

    public void ModifyHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public void RefreshActionPoints()
    {
        CurrentActionPoints = MaxActionPoints;
        Debug.Log($"[CharacterStats] 🔄 AP refreshed to {CurrentActionPoints}");
        OnActionPointsChanged?.Invoke(CurrentActionPoints);
    }

    public void ModifyStrength(int amount) => Strength += amount;
    public void ModifyDexterity(int amount) => Dexterity += amount;
    public void ModifyIntelligence(int amount) => Intelligence += amount;
    public void ModifyLuck(int amount) => Luck += amount;
    public void ModifyBlock(int amount) => Block += amount;
    public void ModifyEnergy(int amount) => Energy += amount;

}
