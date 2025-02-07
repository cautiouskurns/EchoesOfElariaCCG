using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, ICharacter, IEffectTarget
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
            default:
                Debug.LogWarning($"[BaseCharacter] Unknown effect type: {type}");
                break;
        }
    }
}

