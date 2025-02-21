using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewClass", menuName = "Characters/Character Class")]
public class CharacterClass : ScriptableObject
{
    [Header("Class Info")]
    public CharacterClassType classType;
    public string className;
    public Sprite classIcon;
    public string classDescription;

    [Header("Base Stats")] // ✅ No runtime values here!
    public int baseHealth;
    public int baseEnergy;
    public int strength;
    public int dexterity;
    public int intelligence;
    public int luck;
    
    [Header("Starting Data")]
    public List<BaseCard> startingDeck;
    public GameObject classPrefab;  
}

