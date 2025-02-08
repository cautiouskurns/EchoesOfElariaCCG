using UnityEngine;
using System.Collections;

public class EnemyUnit : BaseCharacter
{
    private EnemyAnimationController animationController;

    protected override void Awake()
    {
        base.Awake();
        Name = "Enemy";

        animationController = GetComponentInChildren<EnemyAnimationController>();

        if (animationController == null)
        {
            Debug.LogError("[EnemyUnit] ❌ EnemyAnimationController not found!");
        }
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
}

