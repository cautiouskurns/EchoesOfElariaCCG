using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyUnit : BaseCharacter
{
    [SerializeField] private EnemyIntentUI intentUI;
    private EnemyAnimationController animationController;
    private EnemyClass enemyData;

    // Store the planned action
    public BaseCard plannedCard;
    public PlayerUnit plannedTarget;
    
    // Add a property to store this enemy's specific action deck
    private List<BaseCard> actionDeck = new List<BaseCard>();
    
    // Make this accessible to EnemyAIManager
    public IReadOnlyList<BaseCard> ActionDeck => actionDeck;
    
    public string Description { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Name = "Enemy";

        // Get or find intent UI
        intentUI = GetComponentInChildren<EnemyIntentUI>(true);
        if (intentUI == null)
        {
            Debug.LogError("[EnemyUnit] ❌ EnemyIntentUI not found!");
        }
        else
        {
            // Ensure it starts hidden but initialized
            intentUI.gameObject.SetActive(true);
            intentUI.HideIntent();
        }

        animationController = GetComponentInChildren<EnemyAnimationController>();
        if (animationController == null)
        {
            Debug.LogError("[EnemyUnit] ❌ EnemyAnimationController not found!");
        }
    }

    /// <summary>
    /// Override BaseCharacter's InitializeFromClass to handle enemy-specific adjustments
    /// </summary>
    public override void InitializeFromClass(ICharacterClass characterClass)
    {
        if (!(characterClass is EnemyClass enemyClass))
        {
            Debug.LogError("[EnemyUnit] ❌ Invalid class type passed to EnemyUnit.");
            return;
        }

        enemyData = enemyClass;
        Description = enemyClass.ClassDescription;

        // Call base method to initialize standard stats
        base.InitializeFromClass(enemyClass);

        // Copy the enemy's specific action deck
        if (enemyClass.actionDeck != null && enemyClass.actionDeck.Count > 0)
        {
            actionDeck.Clear();
            actionDeck.AddRange(enemyClass.actionDeck);
            Debug.Log($"[EnemyUnit] Loaded {actionDeck.Count} cards for {Name}");
        }
        else
        {
            Debug.LogWarning($"[EnemyUnit] No action deck defined for {Name}");
        }

        // Apply Elite/Boss Modifiers AFTER Initialization
        if (enemyClass.isElite || enemyClass.isBoss)
        {
            float multiplier = enemyClass.isBoss ? 2.5f : 1.5f;
            Stats.ModifyHealth((int)(Stats.MaxHealth * (multiplier - 1)));  // Increase Health
            Stats.ModifyStrength((int)(Stats.Strength * (multiplier - 1))); // Increase Strength
        }

        // Set up visuals
        if (enemyClass.enemySprite != null)
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = enemyClass.enemySprite;
            }
        }

        Debug.Log($"[EnemyUnit] Initialized {(enemyClass.isBoss ? "Boss" : enemyClass.isElite ? "Elite" : "Normal")} {Name}" +
                  $"\nHP: {Stats.CurrentHealth}/{Stats.MaxHealth}" +
                  $"\nStrength: {Stats.Strength}");
    }

    // Override BaseCharacter methods to use CharacterStats properly
    public override int GetHealth() => Stats.CurrentHealth;
    public override int GetMaxHealth() => Stats.MaxHealth;
    public override int GetStrength() => Stats.Strength;

    public override void TakeDamage(int amount)
    {
        Stats.ModifyHealth(-amount);
        Debug.Log($"[EnemyUnit] {Name} took {amount} damage. HP: {Stats.CurrentHealth}/{Stats.MaxHealth}");

        if (Stats.CurrentHealth <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        Debug.Log($"[EnemyUnit] {Name} has been defeated!");
        // TODO: Add death animation
        Destroy(gameObject);
    }

    public void ShowIntent(BaseCard card)
    {
        if (card == null)
        {
            Debug.LogError("[EnemyUnit] Attempted to show null intent!");
            return;
        }

        if (intentUI == null)
        {
            Debug.LogError("[EnemyUnit] No IntentUI found!");
            return;
        }

        Debug.Log($"[EnemyUnit] Showing intent for {card.CardName}");
        intentUI.ShowIntent(card);
    }

    public void HideIntent()
    {
        intentUI?.HideIntent();
    }

    public IEnumerator AttackPlayer(BaseCharacter player)
    {
        if (animationController == null)
        {
            Debug.LogError("[EnemyUnit] ❌ No Animation Controller found, skipping animation.");
            yield break;
        }

        Debug.Log($"[EnemyUnit] ⚔️ Enemy is attacking {player.Name}!");

        // Play enemy attack animation
        yield return StartCoroutine(animationController.PlayAttackSequence(player.transform.position));

        // Apply damage after attack animation
        player.TakeDamage(5);
        Debug.Log($"[EnemyUnit] 🔥 {player.Name} took 5 damage!");
    }

    // Set the planned action
    public void SetPlannedAction(BaseCard action, PlayerUnit target)
    {
        plannedCard = action;
        plannedTarget = target;
        
        // Display the intent immediately
        ShowIntent(plannedCard);
    }
    
    // Execute the planned action
    public IEnumerator ExecutePlannedAction()
    {
        if (plannedCard == null || plannedTarget == null)
        {
            Debug.LogWarning($"[EnemyUnit] {Name} has no planned action to execute!");
            yield break;
        }
        
        // Execute the planned action
        yield return StartCoroutine(EnemyAIManager.Instance.PerformEnemyAttack(this, plannedTarget, plannedCard));
        
        // Clear the planned action
        plannedCard = null;
        plannedTarget = null;
    }
}