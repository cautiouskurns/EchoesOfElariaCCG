using UnityEngine;
using Cards;

public static class GameStateTracker
{
    public static BaseCard LastCardPlayed { get; private set; }

    public static void SetLastCardPlayed(BaseCard card)
    {
        LastCardPlayed = card;
    }
}
