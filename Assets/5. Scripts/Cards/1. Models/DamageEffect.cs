using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Cards/Effects/Damage")]
public class DamageEffect : CardEffect
{
    public override void ApplyEffect(BaseCharacter target, int value)
    {
        target.TakeDamage(value);
    }
}
