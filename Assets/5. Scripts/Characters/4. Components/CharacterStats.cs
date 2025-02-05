using UnityEngine;
using System;


public class CharacterStats : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private CharacterData characterData;  // ScriptableObject for base stats

    [Header("Base Stats")]
    [SerializeField] private int maxHealth;               // Exposed in Inspector
    [SerializeField] private int startingActionPoints;    // Exposed in Inspector

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int ActionPoints { get; private set; }

    public event Action<int> OnHealthChanged;

    private void Awake()
    {
        ApplyCharacterData();  // Apply values from ScriptableObject
        Initialize(maxHealth, startingActionPoints);
    }

    public void Initialize(int maxHealth, int actionPoints)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        ActionPoints = actionPoints;

        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public void ModifyHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public void UseActionPoints(int amount)
    {
        ActionPoints = Mathf.Max(0, ActionPoints - amount);
    }

    // ✅ Automatically update values in the Inspector during Edit Mode
    private void OnValidate()
    {
        ApplyCharacterData();
    }

    // Apply data from the ScriptableObject to the exposed fields
    private void ApplyCharacterData()
    {
        if (characterData != null)
        {
            maxHealth = characterData.maxHealth;
            startingActionPoints = characterData.startingActionPoints;
        }
    }
}




