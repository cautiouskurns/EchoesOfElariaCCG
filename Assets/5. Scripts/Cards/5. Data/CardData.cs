using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite cardArt;
    public int cost;
    public CardEffectType effectType;
    public int effectValue;
}

public enum CardEffectType
{
    Damage,
    Heal
}


