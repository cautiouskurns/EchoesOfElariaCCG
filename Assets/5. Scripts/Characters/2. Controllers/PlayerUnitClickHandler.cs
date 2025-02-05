using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerUnitClickHandler : MonoBehaviour
{
    private PlayerUnit playerUnit;

    private void Awake()
    {
        playerUnit = GetComponent<PlayerUnit>();
    }

    private void OnMouseDown()
    {
        Debug.Log("[PlayerUnitClickHandler] Click detected");
        if (playerUnit != null)
        {
            Debug.Log($"[PlayerUnitClickHandler] Dealing damage to {playerUnit.Name}");
            playerUnit.TakeDamage(5);  // Deal 5 damage on click for testing
        }
        else
        {
            Debug.LogError("[PlayerUnitClickHandler] PlayerUnit reference is null!");
        }
    }
}
