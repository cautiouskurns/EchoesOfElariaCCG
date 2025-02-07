using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterCombat))]
public class EnemyUnit : BaseCharacter
{
    private CharacterStats stats;
    private CharacterCombat combat;

    protected override void Awake()
    {
        base.Awake();
        Name = "Enemy";

        stats = GetComponent<CharacterStats>();  
        combat = GetComponent<CharacterCombat>();
    }

    public void PerformBasicAttack(ICharacter target)
    {
        Combat.ExecuteAttack(target, 5); // Example damage value
    }
}

