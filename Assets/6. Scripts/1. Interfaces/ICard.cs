using UnityEngine;
using System.Collections.Generic;
using Cards;

public interface ICard
{
    string CardName { get; }
    int Cost { get; }
    Sprite CardArt { get; }
    string Description { get; }
    CardType CardType { get; }

    IReadOnlyList<EffectData> Effects { get; } // ✅ Updated: Returns full effect details
    IReadOnlyList<StatusEffectData> StatusEffects { get; } // ✅ Updated: Returns full status effect details
}