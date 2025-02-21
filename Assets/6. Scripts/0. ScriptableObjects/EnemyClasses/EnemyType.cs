using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Type")]
public class EnemyType : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;
    public string description;
    public GameObject enemyPrefab;
    public Sprite enemySprite;

    [Header("Base Stats")]
    public int maxHealth = 200;
    public int baseStrength = 10;
    public int baseDefense = 5;
    public int speed = 5;
    
    [Header("Battle Properties")]
    public List<BaseCard> actionDeck = new List<BaseCard>();
    public bool isElite = false;
    public bool isBoss = false;
    public int actionsPerTurn = 1;
    
    [Header("Rewards")]
    public int goldValue = 10;
    public List<BaseCard> possibleCardDrops = new List<BaseCard>();
    
    // [Header("Status Effects")]
    // public List<StatusEffect> startingBuffs;
    // public List<StatusEffect> startingDebuffs;
}
