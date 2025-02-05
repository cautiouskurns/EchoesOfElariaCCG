using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Base Stats")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int startingActionPoints = 3;

    [Header("Character Info")]
    [SerializeField] public string characterName;
    [TextArea]
    [SerializeField] public string description;
}
