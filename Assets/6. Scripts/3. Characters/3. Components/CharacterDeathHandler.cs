using UnityEngine;
using System.Collections.Generic;

public class CharacterDeathHandler : MonoBehaviour
{
    private CharacterStats stats;

    private void Awake()
    {
        stats = GetComponent<CharacterStats>();
    }

    public void CheckForDeath()
    {
        if (stats.CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log($"[CharacterDeathHandler] {gameObject.name} has been defeated!");
        Destroy(gameObject, 1f); // ✅ Delay destruction slightly for effects
    }
  
}
