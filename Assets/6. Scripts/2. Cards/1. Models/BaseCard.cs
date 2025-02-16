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
    [SerializeField] private List<int> effectValues;
    [SerializeField] private List<StatusEffectTypes> statusTypes;

    [SerializeField] private GameObject vfxPrefab;  

    private EffectFactory effectFactory;
    private StatusEffectFactory statusEffectFactory;

    public IReadOnlyList<EffectType> EffectTypes => effectTypes;
    public IReadOnlyList<StatusEffectTypes> StatusTypes => statusTypes;

    public string CardName => cardName;
    public int Cost => cost;
    public Sprite CardArt => cardArt;
    public string Description => description;
    public CardType CardType => cardType;
    public AudioClip SoundEffect => soundEffect; 
    public GameObject VFXPrefab => vfxPrefab;

    public List<EffectType> GetEffects() => effectTypes;
    public int GetEffectValue(EffectType effectType)
    {
        int index = effectTypes.IndexOf(effectType);
        return (index >= 0) ? effectValues[index] : 0;  // Default to 0 if not found
    }
    public List<StatusEffectTypes> GetStatusEffects() => statusTypes;
}


