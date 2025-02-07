using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Card Info")]
    [SerializeField] private string cardName;
    [SerializeField] private int cost;
    [SerializeField] private Sprite cardArt;

    [Header("Card Effect")]
    [SerializeField] private int effectValue;  
    [SerializeField, Tooltip("Assign a ScriptableObject that defines the effect. Required!")] 
    private CardEffect cardEffect;

    [Header("Class Bonuses")]
    [SerializeField] private CharacterClass preferredClass;
    [SerializeField] private float classBonus = 1.5f;

    // 🔹 Public Read-Only Properties
    public string CardName => cardName;
    public int Cost => cost;
    public Sprite CardArt => cardArt;
    public int EffectValue => effectValue;
    public CardEffect CardEffect => cardEffect;
    public CharacterClass PreferredClass => preferredClass;
    public float ClassBonus => classBonus;

    // 🔹 Validate in Editor to prevent missing data
    private void OnValidate()
    {
        if (cardEffect == null)
        {
            Debug.LogWarning($"[CardData] ⚠️ Card '{cardName}' is missing a CardEffect!");
        }
    }
}

