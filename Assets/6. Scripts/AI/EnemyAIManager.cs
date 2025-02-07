using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance { get; private set; }

    [SerializeField] private List<CardData> enemyActions;
    [SerializeField] private float actionDelay = 1.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ExecuteEnemyTurn()
    {
        StartCoroutine(ProcessEnemyTurn());
    }

    private IEnumerator ProcessEnemyTurn()
    {
        Debug.Log("[EnemyAI] 🤖 Processing enemy turn");
        
        // Get all active units
        var enemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
        var players = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            enemy.Stats.RefreshActionPoints();
            
            while (enemy.Stats.CurrentActionPoints > 0)
            {
                // Choose random action and target
                var action = enemyActions[Random.Range(0, enemyActions.Count)];
                var target = players[Random.Range(0, players.Length)];

                if (action.Cost <= enemy.Stats.CurrentActionPoints)
                {
                    // Calculate effect with class bonus
                    float multiplier = enemy.Stats.CharacterClass == action.PreferredClass ? action.ClassBonus : 1f;
                    int finalValue = Mathf.RoundToInt(action.EffectValue * multiplier);

                    Debug.Log($"[EnemyAI] {enemy.Name} using {action.CardName} on {target.Name} for {finalValue} damage");
                    
                    action.CardEffect.ApplyEffect(target, finalValue);
                    enemy.Stats.UseActionPoints(action.Cost);
                    
                    yield return new WaitForSeconds(actionDelay);
                }
                else
                {
                    break; // Not enough AP for any action
                }
            }
        }

        Debug.Log("[EnemyAI] Enemy turn complete");
        TurnManager.Instance.EndEnemyTurn();
    }
}
