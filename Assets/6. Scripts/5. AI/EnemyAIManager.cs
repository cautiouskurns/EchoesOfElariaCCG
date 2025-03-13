using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance { get; private set; }

    // Keep this as a fallback pool
    [SerializeField] private List<BaseCard> fallbackEnemyActions;
    [SerializeField] private float actionDelay = 1.5f;

    [SerializeField] private float defensiveThreshold = 0.3f; // Health percentage to trigger defensive play
    [SerializeField] private float aggressiveThreshold = 0.7f; // Health percentage to play aggressively

    private List<BaseCard> lastPlayedCards = new List<BaseCard>();
    private const int maxLastCardsMemory = 3;
    
    // Changed to store per-enemy card categorization
    private Dictionary<EnemyUnit, Dictionary<AICardType, List<BaseCard>>> enemyCardCategories = 
        new Dictionary<EnemyUnit, Dictionary<AICardType, List<BaseCard>>>();
    
    // Enum for card types
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

    private void Start()
    {
        // No need to initialize here anymore - will initialize per enemy when planning
        // Plan actions and immediately set them as the current actions for the first turn
        PlanEnemyActions(true); // Add a parameter to indicate this is the initial planning
    }

    public void PlanEnemyActions(bool isInitialPlanning = false)
    {
        Debug.Log("[EnemyAI] Planning next enemy actions");

        // Get all active enemy and player units
        var enemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
        var players = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            // Initialize categories for this enemy if needed
            if (!enemyCardCategories.ContainsKey(enemy))
            {
                CategorizeAvailableCards(enemy);
            }
            
            // Select action strategically
            BaseCard selectedCard = SelectStrategicAction(enemy, players);
            
            // Select target optimally based on the selected action
            PlayerUnit target = SelectOptimalTarget(players, selectedCard);

            if (selectedCard == null || target == null)
            {
                Debug.LogWarning($"[EnemyAI] ⚠️ No valid action or target could be planned for {enemy.Name}!");
                continue;
            }

            // Store the planned action
            enemy.plannedCard = selectedCard;
            enemy.plannedTarget = target;
            
            // Show the intent immediately
            enemy.ShowIntent(selectedCard);
            
            string timing = isInitialPlanning ? "first turn" : "next turn";
            Debug.Log($"[EnemyAI] {enemy.Name} plans to use {selectedCard.CardName} on {target.Name} on {timing}");
        }
    }

    // Modified to handle per-enemy categorization
    private void CategorizeAvailableCards(EnemyUnit enemy)
    {
        // Create new dictionary for this enemy
        Dictionary<AICardType, List<BaseCard>> categorizedEnemyCards = new Dictionary<AICardType, List<BaseCard>>();
        
        // Initialize categories
        foreach (AICardType type in System.Enum.GetValues(typeof(AICardType)))
        {
            categorizedEnemyCards[type] = new List<BaseCard>();
        }
        
        // Use the enemy's specific deck or fallback
        IEnumerable<BaseCard> cardsToUse = enemy.ActionDeck.Count > 0 
            ? enemy.ActionDeck 
            : fallbackEnemyActions;
        
        if (cardsToUse == null || !cardsToUse.Any())
        {
            Debug.LogError($"[EnemyAI] No cards available for {enemy.Name}");
            enemyCardCategories[enemy] = categorizedEnemyCards;
            return;
        }
        
        // Categorize each card
        foreach (var card in cardsToUse)
        {
            switch (card.CardType)
            {
                case Cards.CardType.Attack:
                    categorizedEnemyCards[AICardType.Attack].Add(card);
                    break;
                case Cards.CardType.Defense:
                case Cards.CardType.Utility:
                    if (card.Description.ToLower().Contains("block") || 
                        card.Description.ToLower().Contains("shield"))
                    {
                        categorizedEnemyCards[AICardType.Defense].Add(card);
                    }
                    break;
            }
            
            // Special case logic for specific cards
            if (card.Description.ToLower().Contains("poison") || 
                card.Description.ToLower().Contains("weak"))
            {
                categorizedEnemyCards[AICardType.Debuff].Add(card);
            }
            
            if (card.Description.ToLower().Contains("heal") || 
                card.Description.ToLower().Contains("restore"))
            {
                categorizedEnemyCards[AICardType.Heal].Add(card);
            }
            
            if (card.Description.ToLower().Contains("strength") || 
                card.Description.ToLower().Contains("buff"))
            {
                categorizedEnemyCards[AICardType.Buff].Add(card);
            }
        }
        
        // Store the categorized cards for this enemy
        enemyCardCategories[enemy] = categorizedEnemyCards;
        
        // Log the result
        Debug.Log($"[EnemyAI] Categorized {cardsToUse.Count()} cards for {enemy.Name}");
        foreach (var category in categorizedEnemyCards)
        {
            Debug.Log($"[EnemyAI] {enemy.Name} - {category.Key}: {category.Value.Count} cards");
        }
    }

    public void ExecuteEnemyTurn()
    {
        StartCoroutine(ProcessEnemyTurn());
    }

    private IEnumerator ProcessEnemyTurn()
    {
        Debug.Log("[EnemyAI] 🤖 Processing enemy turn");

        // Get all active enemy units
        var enemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            Debug.Log($"[EnemyAI] {enemy.Name} starts turn with {enemy.Stats.CurrentActionPoints} AP");
            enemy.Stats.RefreshActionPoints();

            // Execute the previously planned action
            if (enemy.plannedCard != null && enemy.plannedTarget != null)
            {
                BaseCard selectedCard = enemy.plannedCard;
                PlayerUnit target = enemy.plannedTarget;
                AICardType sourceCategory = DetermineCardCategory(enemy, selectedCard);

                // Log the execution
                LogDecisionProcess(enemy, selectedCard, target, 
                    (float)enemy.GetHealth() / enemy.GetMaxHealth(),
                    FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None).Any(p => p.HasDebuff()), 
                    sourceCategory);

                if (selectedCard.Cost <= enemy.Stats.CurrentActionPoints)
                {
                    yield return StartCoroutine(PerformEnemyAttack(enemy, target, selectedCard));
                    
                    // Spend AP after the attack sequence
                    enemy.Stats.UseActionPoints(selectedCard.Cost);
                    
                    // Record for synergy
                    RecordPlayedCard(selectedCard);
                }
                else
                {
                    Debug.Log($"[EnemyAI] ❌ {enemy.Name} does not have enough AP for planned {selectedCard.CardName}");
                }

                // Clear the planned action
                enemy.plannedCard = null;
                enemy.plannedTarget = null;
            }
            else
            {
                Debug.LogWarning($"[EnemyAI] {enemy.Name} had no planned action!");
            }

            yield return new WaitForSeconds(actionDelay);
        }

        // Plan the next actions for the next turn
        PlanEnemyActions();

        Debug.Log("[EnemyAI] ✅ Enemy turn complete");
        TurnManager.Instance.EndEnemyTurn();
    }

    private AICardType DetermineCardCategory(EnemyUnit enemy, BaseCard card)
    {
        if (!enemyCardCategories.ContainsKey(enemy))
        {
            CategorizeAvailableCards(enemy);
        }
        
        var categories = enemyCardCategories[enemy];
        
        foreach (var category in categories)
        {
            if (category.Value.Contains(card))
            {
                return category.Key;
            }
        }
        return AICardType.Attack; // Default
    }

    private BaseCard SelectStrategicAction(EnemyUnit enemy, PlayerUnit[] players)
    {
        // Make sure we have categories for this enemy
        if (!enemyCardCategories.ContainsKey(enemy))
        {
            CategorizeAvailableCards(enemy);
        }
        
        // Get the categories for this enemy
        var categorizedCards = enemyCardCategories[enemy];
        
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

    // Helper method to get random card from a category
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

    // Selects a random action from the enemy's available actions
    private BaseCard SelectRandomAction(EnemyUnit enemy)
    {
        // Use enemy's specific deck or fallback
        var availableCards = enemy.ActionDeck.Count > 0 
            ? enemy.ActionDeck.ToList() 
            : fallbackEnemyActions;
        
        if (availableCards == null || availableCards.Count == 0)
        {
            Debug.LogError($"[EnemyAI] No cards available for {enemy.Name}");
            return null;
        }
        
        return availableCards[Random.Range(0, availableCards.Count)];
    }

    // Selects a random player target
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

    // Moves the enemy, plays attack animation, and applies effect
    public IEnumerator PerformEnemyAttack(EnemyUnit enemy, PlayerUnit target, BaseCard action)
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

        // Move toward the player
        yield return StartCoroutine(animationController.PlayAttackSequence(targetPosition));

        // Apply all effects from BaseCard
        foreach (EffectData effect in action.Effects)
        {
            EffectManager.Instance.ApplySingleEffect(effect, target);
        }

        Debug.Log($"[EnemyAI] 🔥 {target.Name} was hit by {enemy.Name}'s {action.CardName}!");

        RecordPlayedCard(action);

        // Return to original position after attack
        yield return StartCoroutine(animationController.MoveToTarget(animationController.OriginalPosition));

        // Hide intent after attack
        enemy.HideIntent();
    }

    public void ExecuteInitialTurn()
    {
        Debug.Log("[EnemyAI] Setting up initial enemy actions");
        
        var enemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
        var players = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            // Initialize categories for this enemy if needed
            if (!enemyCardCategories.ContainsKey(enemy))
            {
                CategorizeAvailableCards(enemy);
            }
            
            // Select action and target for immediate execution
            BaseCard selectedCard = SelectStrategicAction(enemy, players);
            PlayerUnit target = SelectOptimalTarget(players, selectedCard);

            if (selectedCard != null && target != null)
            {
                // Set as immediate action without showing intent
                enemy.plannedCard = selectedCard;
                enemy.plannedTarget = target;
            }
        }
    }

    // Method for immediate action execution without showing intent first
    public IEnumerator ExecuteImmediateEnemyTurn()
    {
        Debug.Log("[EnemyAI] 🤖 Executing immediate enemy turn");

        // Get all active enemy units
        var enemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
        var players = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            Debug.Log($"[EnemyAI] {enemy.Name} performing first turn action");
            enemy.Stats.RefreshActionPoints();

            // Initialize categories for this enemy if needed
            if (!enemyCardCategories.ContainsKey(enemy))
            {
                CategorizeAvailableCards(enemy);
            }

            // Select an action and target immediately
            BaseCard selectedAction = SelectStrategicAction(enemy, players);
            PlayerUnit target = SelectOptimalTarget(players, selectedAction);

            if (selectedAction != null && target != null)
            {
                AICardType sourceCategory = DetermineCardCategory(enemy, selectedAction);

                LogDecisionProcess(enemy, selectedAction, target, 
                    (float)enemy.GetHealth() / enemy.GetMaxHealth(),
                    players.Any(p => p.HasDebuff()), 
                    sourceCategory);

                if (selectedAction.Cost <= enemy.Stats.CurrentActionPoints)
                {
                    // No intent shown, just execute attack
                    yield return StartCoroutine(PerformEnemyAttack(enemy, target, selectedAction));
                    
                    enemy.Stats.UseActionPoints(selectedAction.Cost);
                    RecordPlayedCard(selectedAction);
                }
            }
            else
            {
                Debug.LogWarning($"[EnemyAI] {enemy.Name} couldn't select an action or target!");
            }

            yield return new WaitForSeconds(actionDelay);
        }

        Debug.Log("[EnemyAI] ✅ Immediate enemy turn complete");
    }
}