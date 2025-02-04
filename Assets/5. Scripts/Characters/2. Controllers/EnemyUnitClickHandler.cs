using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyUnitClickHandler : MonoBehaviour
{
    private EnemyUnit enemyUnit;

    private void Awake()
    {
        enemyUnit = GetComponent<EnemyUnit>();
    }

    private void OnMouseDown()
    {
        if (enemyUnit != null)
        {
            Debug.Log($"Clicked on {enemyUnit.Name}");
            enemyUnit.TakeDamage(5);  // Deal 5 damage on click for testing
        }
    }
}