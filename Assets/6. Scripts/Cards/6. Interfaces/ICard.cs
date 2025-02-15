using UnityEngine;
using Cards;
using System.Collections.Generic;

public interface ICard
{
    string CardName { get; }
    int Cost { get; }
    Sprite CardArt { get; }
    string Description { get; }
    CardType CardType { get; }

    List<EffectType> GetEffects();
    List<StatusType> GetStatusEffects();
}
