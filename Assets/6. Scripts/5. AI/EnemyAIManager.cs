using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance { get; private set; }

    [SerializeField] private List<BaseCard> enemyActions;
    [SerializeField] private float actionDelay = 1.5f;

    [SerializeField] private float defensiveThreshold = 0.3f; // Health percentage to trigger defensive play
    [SerializeField] private float aggressiveThreshold = 0.7f; // Health percentage to play aggressively

    private List<BaseCard> lastPlayedCards = new List<BaseCard>();
    private const int maxLastCardsMemory = 3;

    
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

    private IEnumerator ProcessEnemyTurn()
    {
        Debug.Log("[EnemyAI] 🤖 Processing enemy turn");

        // Get all active enemy and player units
        var enemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
        var players = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            Debug.Log($"[EnemyAI] {enemy.Name} starts turn with {enemy.Stats.CurrentActionPoints} AP");
            enemy.Stats.RefreshActionPoints();

            while (enemy.Stats.CurrentActionPoints > 0)
            {
                // Select action strategically
                AICardType sourceCategory = AICardType.Attack; // Default
                BaseCard selectedCard = SelectStrategicAction(enemy, players);
                
                // Select target optimally based on the selected action
                PlayerUnit target = SelectOptimalTarget(players, selectedCard);

                if (selectedCard == null || target == null)
                {
                    Debug.LogWarning("[EnemyAI] ⚠️ No valid action or target found!");
                    break;
                }

                // Log the complete decision process
                LogDecisionProcess(enemy, selectedCard, target, 
                    (float)enemy.GetHealth() / enemy.GetMaxHealth(),
                    players.Any(p => p.HasDebuff()), 
                    sourceCategory);

                if (selectedCard.Cost > enemy.Stats.CurrentActionPoints)
                {
                    Debug.Log($"[EnemyAI] ❌ {enemy.Name} does not have enough AP for {selectedCard.CardName}");
                    break; // Not enough AP for action
                }

                yield return StartCoroutine(PerformEnemyAttack(enemy, target, selectedCard));

                // Spend AP after the attack sequence
                enemy.Stats.UseActionPoints(selectedCard.Cost);

                yield return new WaitForSeconds(actionDelay);
            }
        }

        Debug.Log("[EnemyAI] ✅ Enemy turn complete");
        TurnManager.Instance.EndEnemyTurn();
    }

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

        // Higher priority checks (health-based decisions)
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

        // ADD SYNERGY CHECK HERE - if not in critical health situation, look for synergies
        if (selectedCard == null && lastPlayedCards.Count > 0)
        {
            // Check for potential synergies across all categories
            foreach (AICardType category in System.Enum.GetValues(typeof(AICardType)))
            {
                if (categorizedCards[category].Count == 0) continue;
                
                foreach (var card in categorizedCards[category])
                {
                    if (card.Cost <= enemy.Stats.CurrentActionPoints && HasSynergyPotential(card))
                    {
                        selectedCard = card;
                        sourceCategory = category;
                        Debug.Log($"[EnemyAI] Choosing {card.CardName} for synergy with previously played cards");
                        break;
                    }
                }
                
                if (selectedCard != null) break;
            }
        }

        // Default to attacks if no synergies or critical situations
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

        return selectedCard;
    }


    private bool HasSynergyPotential(BaseCard card)
    {
        // Look for synergies with recently played cards
        foreach (var pastCard in lastPlayedCards)
        {
            // Check for specific card synergies
            if ((card.CardName.Contains("Lightning") && pastCard.CardName.Contains("Chain")) ||
                (card.CardName.Contains("Poison") && pastCard.CardName.Contains("Dagger")) ||
                (card.CardName.Contains("Fire") && pastCard.CardName.Contains("Combustion")))
            {
                return true;
            }
            
            // Check for type-based synergies
            if ((card.CardType == Cards.CardType.Attack && pastCard.CardType == Cards.CardType.Debuff) ||
                (card.CardType == Cards.CardType.Buff && pastCard.CardType == Cards.CardType.Attack))
            {
                return true;
            }
        }
        
        return false;
    }

    private void RecordPlayedCard(BaseCard card)
    {
        lastPlayedCards.Insert(0, card);
        if (lastPlayedCards.Count > maxLastCardsMemory)
        {
            lastPlayedCards.RemoveAt(lastPlayedCards.Count - 1);
        }
    }

    // In your PerformEnemyAttack method, add:
    // RecordPlayedCard(action);

    // NEW: Helper method to get random card from a category
    private BaseCard GetRandomCard(List<BaseCard> cards)
    {
        if (cards.Count == 0) return null;
        return cards[Random.Range(0, cards.Count)];
    }

    private void LogDecisionProcess(EnemyUnit enemy, BaseCard selectedCard, PlayerUnit target, float healthRatio, bool playersHaveDebuffs, AICardType sourceCategory) 
    {
        // Determine if this was a synergy-based decision
        bool isSynergyBased = lastPlayedCards.Count > 0 && HasSynergyPotential(selectedCard);
        
        string logMessage = $"[EnemyAI Decision] {enemy.Name} ({healthRatio:P0} health) selected {selectedCard.CardName}\n";
        logMessage += $"→ Decision factors: {(healthRatio < defensiveThreshold ? "LOW HEALTH" : healthRatio > aggressiveThreshold ? "HIGH HEALTH" : "NORMAL HEALTH")}\n";
        logMessage += $"→ Player debuffs: {(playersHaveDebuffs ? "YES" : "NO")}\n";
        logMessage += $"→ Card source: {sourceCategory} category\n";
        
        // Add synergy information if applicable
        if (isSynergyBased) {
            logMessage += $"→ SYNERGY DETECTED with previous cards: ";
            for (int i = 0; i < Mathf.Min(lastPlayedCards.Count, 2); i++) {
                logMessage += $"{lastPlayedCards[i].CardName}" + (i < Mathf.Min(lastPlayedCards.Count, 2) - 1 ? ", " : "");
            }
            logMessage += "\n";
        }
        
        logMessage += $"→ Card details: {selectedCard.Cost} AP, {selectedCard.CardType}, \"{selectedCard.Description}\"\n";
        logMessage += $"→ Target: {target.Name} (Health: {(float)target.GetHealth() / target.GetMaxHealth():P0}, Block: {target.Stats.Block}, Debuffed: {target.HasDebuff()})";
        
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


    private PlayerUnit SelectOptimalTarget(PlayerUnit[] players, BaseCard action)
    {
        if (players.Length == 0) return null;
        
        // Default to weakest player
        PlayerUnit weakestTarget = null;
        float lowestHealthRatio = 1.0f;
        
        // Find player with lowest health ratio
        foreach (var player in players)
        {
            float healthRatio = (float)player.GetHealth() / player.GetMaxHealth();
            if (healthRatio < lowestHealthRatio)
            {
                lowestHealthRatio = healthRatio;
                weakestTarget = player;
            }
        }
        
        // Card-specific targeting strategies
        if (action.CardType == Cards.CardType.Attack)
        {
            // Find player that's already vulnerable or debuffed
            foreach (var player in players)
            {
                if (player.HasStatusEffect(StatusEffectTypes.Vulnerable) ||
                    player.HasDebuff())
                {
                    return player; // Target already vulnerable players
                }
            }
            
            // Target players with low block
            PlayerUnit lowestBlockTarget = null;
            int lowestBlock = int.MaxValue;
            
            foreach (var player in players)
            {
                int block = player.Stats.Block;
                if (block < lowestBlock)
                {
                    lowestBlock = block;
                    lowestBlockTarget = player;
                }
            }
            
            if (lowestBlock == 0 && lowestBlockTarget != null)
            {
                return lowestBlockTarget;
            }
        }
        else if (action.Description.ToLower().Contains("debuff") || 
                action.Description.ToLower().Contains("weak"))
        {
            // For debuffs, target players that are not already debuffed
            foreach (var player in players)
            {
                if (!player.HasDebuff())
                {
                    return player;
                }
            }
        }
        
        // Default to weakest player or random if all have full health
        return weakestTarget ?? players[Random.Range(0, players.Length)];
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

        RecordPlayedCard(action);

        // ✅ Return to original position after attack
        yield return StartCoroutine(animationController.MoveToTarget(animationController.OriginalPosition));

        // Hide intent after attack
        enemy.HideIntent();
    }

}
