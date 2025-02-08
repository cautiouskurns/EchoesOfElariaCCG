using UnityEngine;
using Cards;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Card Info")]
    [SerializeField] private string cardName;
    [SerializeField] private int cost;
    [SerializeField] private Sprite cardArt;
    [SerializeField] private string cardDescription;

    [Header("Card Effect")]
    [SerializeField] private int effectValue;  
    [SerializeField, Tooltip("Assign a ScriptableObject that defines the effect. Required!")] 
    private CardEffect cardEffect;

    [Header("Class Bonuses")]
    [SerializeField] private CharacterClass preferredClass;
    [SerializeField] private float classBonus = 1.5f;

    [Header("Card Type")]
    [SerializeField] private CardType cardType;

    [Header("Audio")]
    [SerializeField] private string soundEffectName = CardSoundConfig.CARD_PLAY;
    public string SoundEffectName
    {
        get
        {
            // Override sound based on card type
            if (cardType == CardType.Attack)
                return CardSoundConfig.ATTACK_SLASH;
            else if (cardType == CardType.Spell)
                return CardSoundConfig.FIREBALL;
            // Add other type-specific sounds here
            return soundEffectName;
        }
    }

    // 🔹 Public Read-Only Properties
    public string CardName => cardName;
    public int Cost => cost;
    public Sprite CardArt => cardArt;
    public string CardDescription => cardDescription;
    public int EffectValue => effectValue;
    public CardEffect CardEffect => cardEffect;
    public CharacterClass PreferredClass => preferredClass;
    public float ClassBonus => classBonus;
    public CardType CardType => cardType;

    // 🔹 Validate in Editor to prevent missing data
    private void OnValidate()
    {
        if (cardEffect == null)
        {
            Debug.LogWarning($"[CardData] ⚠️ Card '{cardName}' is missing a CardEffect!");
        }
    }
}

