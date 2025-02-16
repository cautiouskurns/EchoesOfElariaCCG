using UnityEngine;

public enum EffectType
{
    Damage,       // Deals damage to an enemy
    Heal,         // Restores health
    // Strength,     // Increases attack power
    // Block,        // Grants temporary defense
    Energy,       // Grants extra energy
    CardDraw,     // Draws extra cards
    Exhaust,      // Removes a card from play for the rest of combat
    Power         // Applies a persistent effect
}
