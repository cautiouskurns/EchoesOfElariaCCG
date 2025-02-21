using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Type")]
public class EnemyClass : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;
    public string description;
    public GameObject enemyPrefab;
    public Sprite enemySprite;

    [Header("Base Stats")]
    public int maxHealth;
    public int baseStrength;
    public int baseDefense;
    public int speed;
    
    [Header("Battle Properties")]
    public List<BaseCard> actionDeck;
    public bool isElite = false;
    public bool isBoss = false;
    public int actionsPerTurn;
    
    [Header("Rewards")]
    public int goldValue;
    public List<BaseCard> possibleCardDrops;
    
}
