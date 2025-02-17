using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewClass", menuName = "Characters/Character Class")]
public class CharacterClass : ScriptableObject
{
    public CharacterClassType classType;
    public string className;
    public Sprite classIcon;
    public string classDescription;
    public int baseHealth;
    public int baseEnergy;
    public int strength;
    public int dexterity;
    public int intelligence;
    public int luck;
    public List<BaseCard> startingDeck;
}
