using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{

    [Header("Base Stats")]
    [SerializeField] private int maxHealth;               // Exposed in Inspector
    [SerializeField] private int startingActionPoints;    // Exposed in Inspector

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxActionPoints { get; private set; }
    public int CurrentActionPoints { get; private set; }
    public CharacterClassType CharacterClass { get; private set; }
    public float ClassBonus { get; private set; }

    // ✅ Events for Health & Action Points
    public event Action<int> OnHealthChanged;
    public event Action<int> OnActionPointsChanged;

    private void Awake()
    {
        // ApplyCharacterData();  // Apply values from ScriptableObject
        Initialize(maxHealth, startingActionPoints);
    }

    public void Initialize(int maxHealth, int actionPoints)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;

        MaxActionPoints = actionPoints;
        CurrentActionPoints = actionPoints;

        // Trigger UI updates
        OnHealthChanged?.Invoke(CurrentHealth);
        OnActionPointsChanged?.Invoke(CurrentActionPoints);
    }

    // ✅ HEALTH MANAGEMENT
    public void ModifyHealth(int amount)
    {
        int previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        Debug.Log($"[CharacterStats] ❤️ Health changed from {previousHealth} to {CurrentHealth} (Change: {amount})");
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    // ✅ ACTION POINTS MANAGEMENT
    public void UseActionPoints(int amount)
    {
        int previousAP = CurrentActionPoints;
        CurrentActionPoints = Mathf.Max(0, CurrentActionPoints - amount);
        Debug.Log($"[CharacterStats] ⚡ AP changed from {previousAP} to {CurrentActionPoints} (Used: {amount})");
        OnActionPointsChanged?.Invoke(CurrentActionPoints);
    }

    public void RefreshActionPoints()
    {
        CurrentActionPoints = MaxActionPoints;
        Debug.Log($"[CharacterStats] 🔄 Action Points refreshed to {CurrentActionPoints}");
        OnActionPointsChanged?.Invoke(CurrentActionPoints);
    }
}
