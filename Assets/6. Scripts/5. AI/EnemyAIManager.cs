using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance { get; private set; }

    [SerializeField] private List<BaseCard> enemyActions;
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

        if (enemies.Length == 0)
        {
            Debug.LogWarning("[EnemyAI] ⚠️ No enemies found. Skipping enemy turn.");
            TurnManager.Instance.EndEnemyTurn();
            yield break;
        }

        if (players.Length == 0)
        {
            Debug.LogWarning("[EnemyAI] ⚠️ No player units found. Skipping enemy turn.");
            TurnManager.Instance.EndEnemyTurn();
            yield break;
        }

        foreach (var enemy in enemies)
        {
            Debug.Log($"[EnemyAI] 🏴 {enemy.Name} starting turn. AP: {enemy.Stats.CurrentActionPoints}");

            enemy.Stats.RefreshActionPoints();

            while (enemy.Stats.CurrentActionPoints > 0)
            {
                var action = SelectRandomAction(enemy);
                var target = SelectRandomTarget(players);

                if (action == null)
                {
                    Debug.LogWarning($"[EnemyAI] ⚠️ {enemy.Name} has no available actions!");
                    break;
                }

                if (target == null)
                {
                    Debug.LogWarning($"[EnemyAI] ⚠️ {enemy.Name} has no valid target!");
                    break;
                }

                if (action.Cost > enemy.Stats.CurrentActionPoints)
                {
                    Debug.Log($"[EnemyAI] ❌ {enemy.Name} does not have enough AP ({enemy.Stats.CurrentActionPoints}) for {action.CardName} (Cost: {action.Cost})");
                    break; 
                }

                Debug.Log($"[EnemyAI] ⚔️ {enemy.Name} attacking {target.Name} with {action.CardName}");

                yield return StartCoroutine(PerformEnemyAttack(enemy, target, action));

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
    private BaseCard SelectRandomAction(EnemyUnit enemy)
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
    /// <summary>
    /// ✅ Moves the enemy, plays attack animation, and applies effect.
    /// </summary>
    private IEnumerator PerformEnemyAttack(EnemyUnit enemy, PlayerUnit target, BaseCard action)
    {
        Debug.Log($"[EnemyAI] 🎯 {enemy.Name} is attacking {target.Name} with {action.CardName}");

        // Show intent
        enemy.ShowIntent(action);
        yield return new WaitForSeconds(1f);

        // Perform attack animation
        EnemyAnimationController animationController = enemy.GetComponentInChildren<EnemyAnimationController>();
        if (animationController == null)
        {
            Debug.LogError($"[EnemyAI] ❌ No EnemyAnimationController found on {enemy.Name}");
            yield break;
        }

        Vector3 targetPosition = target.transform.position;

        // ✅ Move toward the player
        yield return StartCoroutine(animationController.PlayAttackSequence(targetPosition));

        // ✅ Apply all effects from `BaseCard`
        foreach (EffectType effectType in action.GetEffects()) 
        {
            EffectManager.Instance.ApplyEffect(effectType, target, action); 
        }

        Debug.Log($"[EnemyAI] 🔥 {target.Name} was hit by {enemy.Name}'s {action.CardName}!");

        // ✅ Return to original position after attack
        yield return StartCoroutine(animationController.MoveToTarget(animationController.OriginalPosition));

        // Hide intent after attack
        enemy.HideIntent();
    }

}
