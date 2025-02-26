using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance { get; private set; }

    [SerializeField] private List<BaseCard> enemyActions;
    [SerializeField] private float actionDelay = 1.5f;

    [SerializeField] private float defensiveThreshold = 0.3f; // Health percentage to trigger defensive play
    [SerializeField] private float aggressiveThreshold = 0.7f; // Health percentage to play aggressively
    
    // NEW: Added card categorization
    private Dictionary<AICardType, List<BaseCard>> categorizedCards = new Dictionary<AICardType, List<BaseCard>>();
    
    // NEW: Added enum for card types
    private enum AICardType
    {
        Attack,
        Defense,
        Debuff,
        Heal,
        Buff
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // NEW: Modified Start method to initialize card categories
    private void Start()
    {
        CategorizeAvailableCards();
    }

    // NEW: Added method to categorize cards
    private void CategorizeAvailableCards()
    {
        // Initialize categories
        foreach (AICardType type in System.Enum.GetValues(typeof(AICardType)))
        {
            categorizedCards[type] = new List<BaseCard>();
        }
        
        // Categorize each card
        foreach (var card in enemyActions)
        {
            switch (card.CardType)
            {
                case Cards.CardType.Attack:
                    categorizedCards[AICardType.Attack].Add(card);
                    break;
                case Cards.CardType.Defense:
                case Cards.CardType.Utility:
                    if (card.Description.ToLower().Contains("block") || 
                        card.Description.ToLower().Contains("shield"))
                    {
                        categorizedCards[AICardType.Defense].Add(card);
                    }
                    break;
            }
            
            // Special case logic for specific cards
            if (card.Description.ToLower().Contains("poison") || 
                card.Description.ToLower().Contains("weak"))
            {
                categorizedCards[AICardType.Debuff].Add(card);
            }
            
            if (card.Description.ToLower().Contains("heal") || 
                card.Description.ToLower().Contains("restore"))
            {
                categorizedCards[AICardType.Heal].Add(card);
            }
            
            if (card.Description.ToLower().Contains("strength") || 
                card.Description.ToLower().Contains("buff"))
            {
                categorizedCards[AICardType.Buff].Add(card);
            }
        }
    }

    public void ExecuteEnemyTurn()
    {
        StartCoroutine(ProcessEnemyTurn());
    }

    // UPDATED: Modified to use strategic action selection
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
                // CHANGED: Now using strategic action selection instead of random
                var action = SelectStrategicAction(enemy, players);
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

    // NEW: Strategic action selection method
    private BaseCard SelectStrategicAction(EnemyUnit enemy, PlayerUnit[] players)
    {
        // Calculate health ratio
        float healthRatio = (float)enemy.GetHealth() / enemy.GetMaxHealth();
        
        // Check player status effects to inform decision
        bool anyPlayerHasDebuff = false;
        PlayerUnit weakestPlayer = null;
        float lowestHealthRatio = 1.0f;
        
        foreach (var player in players)
        {
            if (player.HasDebuff())
            {
                anyPlayerHasDebuff = true;
            }
            
            float playerHealthRatio = (float)player.GetHealth() / player.GetMaxHealth();
            if (playerHealthRatio < lowestHealthRatio)
            {
                lowestHealthRatio = playerHealthRatio;
                weakestPlayer = player;
            }
        }
        
        // Strategic decision making
        AICardType sourceCategory = AICardType.Attack; // Default category
        BaseCard selectedCard = null;

        if (healthRatio < defensiveThreshold)
        {
            // Low health - prioritize defense or healing
            if (categorizedCards[AICardType.Heal].Count > 0)
            {
                selectedCard = GetRandomCard(categorizedCards[AICardType.Heal]);
                sourceCategory = AICardType.Heal;
            }
            else if (categorizedCards[AICardType.Defense].Count > 0)
            {
                selectedCard = GetRandomCard(categorizedCards[AICardType.Defense]);
                sourceCategory = AICardType.Defense;
            }
        }
        else if (healthRatio > aggressiveThreshold && !anyPlayerHasDebuff)
        {
            // High health and players aren't debuffed - prioritize debuffs or attacks
            if (categorizedCards[AICardType.Debuff].Count > 0)
            {
                selectedCard = GetRandomCard(categorizedCards[AICardType.Debuff]);
                sourceCategory = AICardType.Debuff;
            }
        }

        // Default to attacks
        if (selectedCard == null && categorizedCards[AICardType.Attack].Count > 0)
        {
            selectedCard = GetRandomCard(categorizedCards[AICardType.Attack]);
            sourceCategory = AICardType.Attack;
        }

        // Fallback to any available card
        if (selectedCard == null)
        {
            selectedCard = SelectRandomAction(enemy);
            sourceCategory = AICardType.Attack; // Assuming random is likely an attack
        }

        // Log the decision process
        if (selectedCard != null)
        {
            LogDecisionProcess(enemy, selectedCard, healthRatio, anyPlayerHasDebuff, sourceCategory);
        }

        return selectedCard;
    }

    // NEW: Helper method to get random card from a category
    private BaseCard GetRandomCard(List<BaseCard> cards)
    {
        if (cards.Count == 0) return null;
        return cards[Random.Range(0, cards.Count)];
    }

    private void LogDecisionProcess(EnemyUnit enemy, BaseCard selectedCard, float healthRatio, bool playersHaveDebuffs, AICardType sourceCategory) 
    {
        string logMessage = $"[EnemyAI Decision] {enemy.Name} ({healthRatio:P0} health) selected {selectedCard.CardName}\n";
        logMessage += $"→ Decision factors: {(healthRatio < defensiveThreshold ? "LOW HEALTH" : healthRatio > aggressiveThreshold ? "HIGH HEALTH" : "NORMAL HEALTH")}\n";
        logMessage += $"→ Player debuffs: {(playersHaveDebuffs ? "YES" : "NO")}\n";
        logMessage += $"→ Card source: {sourceCategory} category\n";
        logMessage += $"→ Card details: {selectedCard.Cost} AP, {selectedCard.CardType}, \"{selectedCard.Description}\"";
        
        Debug.Log(logMessage);
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
        foreach (EffectData effect in action.Effects)  // ✅ FIXED
        {
            EffectManager.Instance.ApplySingleEffect(effect, target);  // ✅ FIXED
        }

        Debug.Log($"[EnemyAI] 🔥 {target.Name} was hit by {enemy.Name}'s {action.CardName}!");

        // ✅ Return to original position after attack
        yield return StartCoroutine(animationController.MoveToTarget(animationController.OriginalPosition));

        // Hide intent after attack
        enemy.HideIntent();
    }

}
