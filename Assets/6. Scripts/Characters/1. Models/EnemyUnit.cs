using UnityEngine;
using System.Collections;

public class EnemyUnit : BaseCharacter
{
    [SerializeField] private EnemyIntentUI intentUI;
    private EnemyAnimationController animationController;

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

