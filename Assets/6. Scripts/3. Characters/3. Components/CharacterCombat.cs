using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    public void ExecuteAttack(ICharacter target, int damage)
    {
        target.TakeDamage(damage);
    }
}
