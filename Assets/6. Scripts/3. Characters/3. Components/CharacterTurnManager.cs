using UnityEngine;
using System.Collections.Generic;

public class CharacterTurnManager : MonoBehaviour
{
    private CharacterEffects effects;

    private void Awake()
    {
        effects = GetComponent<CharacterEffects>();
    }

    public void EndTurn()
    {
        Debug.Log($"[CharacterTurnManager] Ending turn...");

        effects.ProcessEndOfTurnEffects(); // ✅ Reduce effect durations
    }
}
