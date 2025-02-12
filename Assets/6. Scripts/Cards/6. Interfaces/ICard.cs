using UnityEngine;
using Cards;

public interface ICard
{
    string CardName { get; }
    int Cost { get; }
    Sprite CardArt { get; }
    string Description { get; }
    CardType CardType { get; }

    void Play(IEffectTarget target);
}
