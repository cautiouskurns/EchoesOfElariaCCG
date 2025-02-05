using UnityEngine;

public class CardBehavior : MonoBehaviour
{
    public CardData cardData;

    public void PlayCard(BaseCharacter target)
    {
        Debug.Log($"Playing {cardData.cardName} on {target.Name}");

        if (cardData.effectType == CardEffectType.Damage)
        {
            target.TakeDamage(cardData.effectValue);
        }
        else if (cardData.effectType == CardEffectType.Heal)
        {
            target.Heal(cardData.effectValue);
        }
    }
}


