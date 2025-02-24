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
    public EffectTarget target; 
    public ConditionType condition;  
    public int conditionValue;
}

// ✅ New Status Effect Struct
[System.Serializable]
public struct StatusEffectData
{
    public StatusEffectTypes statusType;
    public int duration;
    public int intensity;
    public EffectTarget target;
    public ConditionType conditionType; 
    public int conditionValue;
}

// ✅ New Target Enum
public enum EffectTarget
{
    Self,
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies,
    AllUnits
}

public enum ConditionType
{
    None,               // Always applies
    LastCardWasAttack,  // ✅ Checks if the last played card was an attack
    TargetIsWeak,    // ✅ Applies effect if target has a Stunned status
    PlayerBelowHP,      // ✅ Applies effect if player is below a certain HP threshold
    HasBuff,            // ✅ Applies if the player has a Strength/Defense buff
    HasDebuff,          // ✅ Applies if the player has a weakness debuff
    HasStatusEffect,
    TargetIsStunned,
    PlayerHealthBelowThreshold,
    EnemyHealthBelowThreshold
}
