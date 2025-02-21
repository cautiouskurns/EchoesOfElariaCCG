using UnityEngine;

public interface ICharacterClass
{
    string ClassName { get; }
    Sprite ClassIcon { get; }
    string ClassDescription { get; }
    int BaseHealth { get; }
    int BaseEnergy { get; }
    int Strength { get; }
    int Dexterity { get; }
    int Intelligence { get; }
    int Luck { get; }
}
