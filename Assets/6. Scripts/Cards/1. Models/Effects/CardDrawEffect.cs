using UnityEngine;

[CreateAssetMenu(fileName = "CardDrawEffect", menuName = "Cards/Effects/CardDraw")]
public class CardDrawEffect : CardEffect
{
    public override void ApplyEffect(IEffectTarget target, int value)
    {
        target.ReceiveEffect(value, EffectType.CardDraw); // This will call the ReceiveEffect method in the target
    }
}
