using UnityEngine;

public class EnemyUnitClickHandler : MonoBehaviour
{
    private EnemyUnit enemyUnit;

    private void Awake()
    {
        enemyUnit = GetComponent<EnemyUnit>();
        if (enemyUnit == null)
        {
            Debug.LogError("[EnemyUnitClickHandler] ❌ EnemyUnit component not found on " + gameObject.name);
        }
    }

    private void OnMouseDown()
    {
        if (enemyUnit == null)
        {
            Debug.LogWarning("[EnemyUnitClickHandler] ⚠️ EnemyUnit is null.");
            return;
        }

        // Check if a card is selected
        CardBehavior selectedCard = CardBehavior.GetSelectedCard();
        if (selectedCard != null)
        {
            Debug.Log($"[EnemyUnitClickHandler] 🎯 Using {selectedCard.cardData.cardName} on {enemyUnit.Name}");
            selectedCard.PlayCard(enemyUnit); // Apply card effect
        }
        else
        {
            Debug.Log($"[EnemyUnitClickHandler] 👊 No card selected. Dealing 5 test damage to {enemyUnit.Name}.");
            enemyUnit.TakeDamage(5); // Default behavior for testing
        }
    }
}
