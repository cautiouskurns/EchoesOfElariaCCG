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
        if (playerUnit != null)
        {
            Debug.Log($"Clicked on {playerUnit.Name}");
            playerUnit.TakeDamage(5);  // Deal 5 damage on click for testing
        }
    }
}
