using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int cost;
    public Sprite cardArt; // Changed Image to Sprite
    public int effectValue;  // Example: Damage amount or heal amount
    public CardEffect cardEffect;  // Reference to the CardEffect ScriptableObject
}


public enum CardEffectType
{
    Damage,
    Heal
}


