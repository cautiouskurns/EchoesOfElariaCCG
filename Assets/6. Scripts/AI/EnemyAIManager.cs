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

        // Get all active enemy and player units
        var enemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
        var players = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            enemy.Stats.RefreshActionPoints();

            while (enemy.Stats.CurrentActionPoints > 0)
            {
                // Select an action and a target
                var action = SelectRandomAction(enemy);
                var target = SelectRandomTarget(players);

                if (action == null || target == null)
                {
                    Debug.LogWarning("[EnemyAI] ⚠️ No valid action or target found!");
                    break;
                }

                if (action.Cost > enemy.Stats.CurrentActionPoints)
                {
                    Debug.Log($"[EnemyAI] ❌ {enemy.Name} does not have enough AP for {action.CardName}");
                    break; // Not enough AP for action
                }

                yield return StartCoroutine(PerformEnemyAttack(enemy, target, action));

                // Spend AP after the attack sequence
                enemy.Stats.UseActionPoints(action.Cost);

                yield return new WaitForSeconds(actionDelay);
            }
        }

        Debug.Log("[EnemyAI] ✅ Enemy turn complete");
        TurnManager.Instance.EndEnemyTurn();
    }

    /// <summary>
    /// ✅ Selects a random action from the enemy's available actions.
    /// </summary>
    private CardData SelectRandomAction(EnemyUnit enemy)
    {
        if (enemyActions.Count == 0) return null;
        return enemyActions[Random.Range(0, enemyActions.Count)];
    }

    /// <summary>
    /// ✅ Selects a random player target.
    /// </summary>
    private PlayerUnit SelectRandomTarget(PlayerUnit[] players)
    {
        if (players.Length == 0) return null;
        return players[Random.Range(0, players.Length)];
    }

    /// <summary>
    /// ✅ Moves the enemy, plays attack animation, and applies effect.
    /// </summary>
    private IEnumerator PerformEnemyAttack(EnemyUnit enemy, PlayerUnit target, CardData action)
    {
        Debug.Log($"[EnemyAI] 🎯 {enemy.Name} is attacking {target.Name} with {action.CardName}");

        // Show intent
        enemy.ShowIntent(action);
        yield return new WaitForSeconds(1f);

        // Perform attack
        EnemyAnimationController animationController = enemy.GetComponentInChildren<EnemyAnimationController>();
        if (animationController == null)
        {
            Debug.LogError($"[EnemyAI] ❌ No EnemyAnimationController found on {enemy.Name}");
            yield break;
        }

        Vector3 targetPosition = target.transform.position;

        // ✅ Move toward the player (Dash)
        yield return StartCoroutine(animationController.PlayAttackSequence(targetPosition));

        // ✅ Calculate final effect value
        float multiplier = enemy.Stats.CharacterClass == action.PreferredClass ? action.ClassBonus : 1f;
        int finalValue = Mathf.RoundToInt(action.EffectValue * multiplier);

        // ✅ Apply the card effect
        action.CardEffect.ApplyEffect(target, finalValue);
        Debug.Log($"[EnemyAI] 🔥 {target.Name} took {finalValue} damage from {enemy.Name}!");

        // ✅ Return to original position after attack
        yield return StartCoroutine(animationController.MoveToTarget(animationController.OriginalPosition));

        // Hide intent after attack
        enemy.HideIntent();
    }
}
