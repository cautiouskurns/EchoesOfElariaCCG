using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerUnit))]
public class PlayerUnitClickHandler : MonoBehaviour
{
    private PlayerUnit playerUnit;

    private void Awake()
    {
        playerUnit = GetComponent<PlayerUnit>();
    }

    private void OnMouseDown()
    {
        if (playerUnit == null) return;

        if (!playerUnit.IsSelected)
        {
            playerUnit.Select();
        }
        else
        {
            Debug.Log($"[PlayerUnitClickHandler] 👇 Deselecting {playerUnit.Name}");
            playerUnit.Deselect();
        }
    }
}
