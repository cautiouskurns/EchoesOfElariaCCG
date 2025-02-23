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

    [SerializeField] private List<EffectData> effects;  // ✅ Updated Effects Storage
    [SerializeField] private List<StatusEffectData> statusEffects; // ✅ Updated Status Storage

    [SerializeField] private GameObject vfxPrefab;  

    public string CardName => cardName;
    public int Cost => cost;
    public Sprite CardArt => cardArt;
    public string Description => description;
    public CardType CardType => cardType;
    public AudioClip SoundEffect => soundEffect; 
    public GameObject VFXPrefab => vfxPrefab;

    public IReadOnlyList<EffectData> Effects => effects;  // ✅ New Effect Getter
    public IReadOnlyList<StatusEffectData> StatusEffects => statusEffects;  // ✅ New Status Effect Getter
}

// ✅ New Effect Data Struct
[System.Serializable]
public struct EffectData
{
    public EffectType effectType;
    public int value;
    public EffectTarget target; // Specifies where the effect applies
}

// ✅ New Status Effect Struct
[System.Serializable]
public struct StatusEffectData
{
    public StatusEffectTypes statusType;
    public int duration;
    public int intensity;
    public EffectTarget target;
}

// ✅ New Target Enum
public enum EffectTarget
{
    Self,
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies
}



// using UnityEngine;
// using System.Collections.Generic;
// using Cards;

// [CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
// public class BaseCard : ScriptableObject, ICard
// {
//     [SerializeField] private string cardName;
//     [SerializeField] private int cost;
//     [SerializeField] private Sprite cardArt;
//     [SerializeField] private string description;
//     [SerializeField] private CardType cardType;
//     [SerializeField] private AudioClip soundEffect; 


//     [SerializeField] private List<EffectType> effectTypes;
//     [SerializeField] private List<int> effectValues;
//     [SerializeField] private List<StatusEffectTypes> statusTypes;

//     [SerializeField] private GameObject vfxPrefab;  

//     private EffectFactory effectFactory;
//     private StatusEffectFactory statusEffectFactory;

//     public IReadOnlyList<EffectType> EffectTypes => effectTypes;
//     public IReadOnlyList<StatusEffectTypes> StatusTypes => statusTypes;

//     public string CardName => cardName;
//     public int Cost => cost;
//     public Sprite CardArt => cardArt;
//     public string Description => description;
//     public CardType CardType => cardType;
//     public AudioClip SoundEffect => soundEffect; 
//     public GameObject VFXPrefab => vfxPrefab;

//     public List<EffectType> GetEffects() => effectTypes;
//     public int GetEffectValue(EffectType effectType)
//     {
//         int index = effectTypes.IndexOf(effectType);
//         return (index >= 0) ? effectValues[index] : 0;  // Default to 0 if not found
//     }
//     public List<StatusEffectTypes> GetStatusEffects() => statusTypes;
// }


// [System.Serializable]
// public struct EffectData
// {
//     public EffectType effectType;
//     public int value;
//     public EffectTarget target; // New field to determine who receives the effect
// }

// [System.Serializable]
// public struct StatusEffectData
// {
//     public StatusEffectTypes statusType;
//     public int duration;
//     public int intensity;
//     public EffectTarget target;
// }

// public enum EffectTarget
// {
//     Self,
//     SingleEnemy,
//     AllEnemies,
//     SingleAlly,
//     AllAllies
// }