using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Type")]
public class EnemyClass : ScriptableObject, ICharacterClass
{
    [Header("Identity")]
    public string enemyName;
    public string classDescription;
    public GameObject enemyPrefab;
    public Sprite enemySprite;

    [Header("Base Stats")]
    public int baseHealth;
    public int baseEnergy;
    public int strength;
    public int dexterity;
    public int intelligence;
    public int luck;
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

    public string ClassName => enemyName;
    public string ClassDescription => classDescription;
    public Sprite ClassIcon => enemySprite;
    public int BaseHealth => baseHealth;
    public int BaseEnergy => baseEnergy;
    public int Strength => strength;
    public int Dexterity => dexterity;
    public int Intelligence => intelligence;
    public int Luck => luck;
    
}
