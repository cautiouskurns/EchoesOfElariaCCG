using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int ActionPoints { get; private set; }

    public void Initialize(int maxHealth, int actionPoints)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        ActionPoints = actionPoints;
    }

    public void ModifyHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
    }

    public void UseActionPoints(int amount)
    {
        ActionPoints = Mathf.Max(0, ActionPoints - amount);
    }
}

