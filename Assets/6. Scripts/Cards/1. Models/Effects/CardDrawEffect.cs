using UnityEngine;

[CreateAssetMenu(fileName = "CardDrawEffect", menuName = "Cards/Effects/CardDraw")]
public class CardDrawEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ApplyEffect(value, EffectType.CardDraw);
    }
}
