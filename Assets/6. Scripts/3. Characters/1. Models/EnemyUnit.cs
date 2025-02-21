using UnityEngine;
using System.Collections;

public class EnemyUnit : BaseCharacter
{
    [SerializeField] private EnemyIntentUI intentUI;
    private EnemyAnimationController animationController;
    private EnemyType enemyData;
    private int currentHealth;
    private int currentStrength;
    private int currentDefense;
    private int currentSpeed;
    public string Description { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Name = "Enemy";

        // Get or find intent UI
        if (intentUI == null)
        {
            intentUI = GetComponentInChildren<EnemyIntentUI>(true); // Include inactive objects
        }
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

    public void InitializeFromType(EnemyType type)
    {
        enemyData = type;
        Name = type.enemyName;
        Description = type.description;

        // Initialize stats directly from ScriptableObject
        currentHealth = type.maxHealth;
        currentStrength = type.baseStrength;
        currentDefense = type.baseDefense;
        currentSpeed = type.speed;

        // Apply elite/boss modifiers if needed
        if (type.isElite || type.isBoss)
        {
            float multiplier = type.isBoss ? 2.5f : 1.5f;
            currentHealth = Mathf.RoundToInt(currentHealth * multiplier);
            currentStrength = Mathf.RoundToInt(currentStrength * multiplier);
        }

        // Set up visuals
        if (type.enemySprite != null)
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = type.enemySprite;
            }
        }

        Debug.Log($"[EnemyUnit] Initialized {(type.isBoss ? "Boss" : type.isElite ? "Elite" : "")} {Name}" +
                 $"\nHP: {currentHealth}/{type.maxHealth}" +
                 $"\nStrength: {currentStrength}");
    }

    // Override base character methods to use our own stat tracking
    public override int GetHealth() => currentHealth;
    public override int GetMaxHealth() => enemyData != null ? enemyData.maxHealth : 0;
    public override int GetStrength() => currentStrength;
    public override int GetDefense() => currentDefense;

    public override void TakeDamage(int amount)
    {
        int previousHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        Debug.Log($"[EnemyUnit] {Name} took {amount} damage. HP: {currentHealth}/{GetMaxHealth()}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        Debug.Log($"[EnemyUnit] {Name} has been defeated!");
        // TODO: Add death animation
        Destroy(gameObject);
    }

    public void ShowIntent(BaseCard card)
    {
        intentUI?.ShowIntent(card);
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

        // ✅ Play enemy attack animation
        yield return StartCoroutine(animationController.PlayAttackSequence(player.transform.position));

        // ✅ Apply damage after attack animation
        player.TakeDamage(5);
        Debug.Log($"[EnemyUnit] 🔥 {player.Name} took 5 damage!");
    }

    private void OnValidate()
    {
        if (intentUI == null)
        {
            intentUI = GetComponentInChildren<EnemyIntentUI>();
            if (intentUI == null)
            {
                Debug.LogWarning("[EnemyUnit] ⚠️ EnemyIntentUI component needs to be assigned!");
            }
        }
    }
}

