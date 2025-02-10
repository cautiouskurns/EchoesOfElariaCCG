using UnityEngine;
using System.Collections.Generic;
using Cards;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Card Info")]
    [SerializeField] private string cardName;
    [SerializeField] private int cost;
    [SerializeField] private Sprite cardArt;
    [SerializeField] private string cardCategory;
    [SerializeField] private string cardDescription;

    [Header("Card Effect")]
    [SerializeField] private int effectValue;  
    [SerializeField, Tooltip("Assign a ScriptableObject that defines the effect. Required!")] 
    private CardEffect cardEffect;

    [Header("Status Effects (Optional)")]  
    [SerializeField] private List<StatusEffectData> statusEffects = new List<StatusEffectData>();  // Initialize list

    [Header("Class Bonuses")]
    [SerializeField] private CharacterClass preferredClass;
    [SerializeField] private float classBonus = 1.5f;

    [Header("Card Type")]
    [SerializeField] private CardType cardType;

    [Header("Audio")]  
    [SerializeField] private AudioClip soundEffect;  

    // 🔹 Public Read-Only Properties
    public string CardName => cardName;
    public int Cost => cost;
    public Sprite CardArt => cardArt;
    public string CardDescription => cardDescription;
    public int EffectValue => effectValue;
    public CardEffect CardEffect => cardEffect;
    public List<StatusEffectData> StatusEffects => statusEffects;  // ✅ Getter for status effects
    public CharacterClass PreferredClass => preferredClass;
    public float ClassBonus => classBonus;
    public CardType CardType => cardType;
    public AudioClip SoundEffect => soundEffect;

    // 🔹 Validate in Editor to prevent missing data
    private void OnValidate()
    {
        if (cardEffect == null)
        {
            Debug.LogWarning($"[CardData] ⚠️ Card '{cardName}' is missing a CardEffect!");
        }

        if (statusEffects == null)
        {
            statusEffects = new List<StatusEffectData>();
            Debug.Log($"[CardData] Initialized status effects list for card '{cardName}'");
        }
    }
}

