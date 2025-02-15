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
    [SerializeField] private AudioClip soundEffect; 


    [SerializeField] private List<EffectType> effectTypes;
    [SerializeField] private List<StatusType> statusTypes;

    private EffectFactory effectFactory;
    private StatusEffectFactory statusEffectFactory;

    public IReadOnlyList<EffectType> EffectTypes => effectTypes;
    public IReadOnlyList<StatusType> StatusTypes => statusTypes;

    private void EnsureFactoriesInitialized()
    {
        if (effectFactory == null)
            effectFactory = FindFirstObjectByType<EffectFactory>();

        if (statusEffectFactory == null)
            statusEffectFactory = FindFirstObjectByType<StatusEffectFactory>();
    }

    public string CardName => cardName;
    public int Cost => cost;
    public Sprite CardArt => cardArt;
    public string Description => description;
    public CardType CardType => cardType;
    public AudioClip SoundEffect => soundEffect; 

    public List<EffectType> GetEffects() => effectTypes;
    public List<StatusType> GetStatusEffects() => statusTypes;
}


