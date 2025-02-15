using UnityEngine;
using System.Collections.Generic;
using Cards;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
public class BaseCard : ScriptableObject, ICard
{
    [SerializeField] private string cardName;
    [SerializeField] private int cost;
    [SerializeField] private Sprite cardArt;
    [SerializeField] private string description;
    [SerializeField] private CardType cardType;

    [SerializeField] private List<EffectType> effectTypes = new List<EffectType>();  // ✅ Store effect types instead of effect objects
    [SerializeField] private List<StatusType> statusTypes = new List<StatusType>();  // ✅ Store status effect types instead of effect objects

    // Add public properties to access the lists
    public IReadOnlyList<EffectType> EffectTypes => effectTypes;
    public IReadOnlyList<StatusType> StatusTypes => statusTypes;

    private EffectFactory effectFactory;
    private StatusEffectFactory statusEffectFactory;

    private void Awake()
    {
        effectFactory = FindFirstObjectByType<EffectFactory>();
        statusEffectFactory = FindFirstObjectByType<StatusEffectFactory>();
    }

    public string CardName => cardName;
    public int Cost => cost;
    public Sprite CardArt => cardArt;
    public string Description => description;
    public CardType CardType => cardType;

    public void Play(IEffectTarget target)
    {
        // ✅ Apply main effects
        foreach (var type in effectTypes)
        {
            BaseEffect effect = effectFactory.CreateEffect(type);
            if (effect != null)
            {
                effect.ApplyEffect(target, effect.BaseValue);
            }
        }

        // ✅ Apply status effects
        foreach (var statusType in statusTypes)
        {
            BaseStatusEffect statusEffect = statusEffectFactory.CreateStatusEffect(statusType);
            if (statusEffect != null)
            {
                statusEffect.ApplyStatus(target, statusEffect.MaxDuration);
            }
        }
    }
}

