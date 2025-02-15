using UnityEngine;

public enum EffectType
{
    Damage,       // Deals damage to an enemy
    Heal,         // Restores health
    Strength,     // Increases attack power
    Energy,       // Grants extra energy
    CardDraw,     // Draws extra cards
    Exhaust,      // Removes a card from play for the rest of combat
    Power         // Applies a persistent effect
}
