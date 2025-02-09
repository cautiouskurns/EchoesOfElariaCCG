using UnityEngine;

public enum EffectType
{
    Damage,       // Deals damage to an enemy
    Heal,         // Restores health
    Block,        // Grants temporary protection from damage
    Strength,     // Increases attack power
    Weak,         // Reduces attack power
    Vulnerable,   // Increases damage taken
    Energy,       // Grants extra energy
    CardDraw,     // Draws extra cards
    Exhaust,      // Removes a card from play for the rest of combat
    PowerEffect,  // Applies a persistent effect
    StatusEffect  // Generic category for unique effects
}
