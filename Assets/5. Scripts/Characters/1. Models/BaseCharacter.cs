using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, ICharacter
{
    public string Name { get; protected set; }
    public CharacterStats Stats { get; private set; }
    public CharacterCombat Combat { get; private set; }

    protected virtual void Awake()
    {
        Stats = GetComponent<CharacterStats>();
        Combat = GetComponent<CharacterCombat>();
    }

    public virtual void TakeDamage(int damage) => Stats.ModifyHealth(-damage);
    public virtual void Heal(int amount) => Stats.ModifyHealth(amount);
    public virtual void UseActionPoints(int amount) => Stats.UseActionPoints(amount);
}

