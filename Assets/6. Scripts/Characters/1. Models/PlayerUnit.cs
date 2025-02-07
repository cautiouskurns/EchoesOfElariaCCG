using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterCombat))]
public class PlayerUnit : BaseCharacter
{
    private CharacterStats stats;
    private CharacterCombat combat;
    
    protected override void Awake()
    {
        base.Awake();
        Name = "Player";

        stats = GetComponent<CharacterStats>();  
        combat = GetComponent<CharacterCombat>();
    }
}
