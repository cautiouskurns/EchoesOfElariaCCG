using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerUnit))]
public class PlayerUnitClickHandler : MonoBehaviour
{
    private PlayerUnit playerUnit;

    private void Awake()
    {
        playerUnit = GetComponent<PlayerUnit>();
        if (playerUnit == null)
        {
            Debug.LogError("[PlayerUnitClickHandler] ❌ No PlayerUnit found!");
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"[PlayerUnitClickHandler] Mouse clicked on {gameObject.name}");

        if (playerUnit == null)
        {
            Debug.LogError("[PlayerUnitClickHandler] ❌ PlayerUnit reference is NULL!");
            return;
        }

        if (!playerUnit.Selection.IsSelected)
        {
            Debug.Log($"[PlayerUnitClickHandler] ✅ Selecting {playerUnit.Name}");
            playerUnit.Select();
        }
        else
        {
            Debug.Log($"[PlayerUnitClickHandler] 👇 Deselecting {playerUnit.Name}");
            playerUnit.Deselect();
        }
    }
}
