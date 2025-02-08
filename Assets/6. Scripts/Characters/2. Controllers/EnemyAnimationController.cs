using UnityEngine;
using System.Collections;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 3f;
    private Vector3 originalPosition;

    public Vector3 OriginalPosition => originalPosition;

    private void Awake()
    {
        // ✅ Automatically find the Animator if it's on a child
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError($"[EnemyAnimationController] ❌ Animator not found on {gameObject.name} or its children!");
        }

        originalPosition = transform.position; // Store initial position
    }

    /// <summary>
    /// ✅ Moves the enemy to a target position.
    /// </summary>
    public IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 1.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// ✅ Plays the attack sequence: Move → Attack → Return.
    /// </summary>
    public IEnumerator PlayAttackSequence(Vector3 playerPosition)
    {
        if (animator == null)
        {
            Debug.LogError("[EnemyAnimationController] ❌ No Animator found!");
            yield break;
        }

        Vector3 startPosition = transform.position;

        // ✅ Move toward the player
        animator.SetTrigger("Dash");

        while (Vector3.Distance(transform.position, playerPosition) > 1.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // ✅ Attack as soon as enemy reaches player
        animator.SetTrigger("AttackStrike");
        Debug.Log("[EnemyAnimationController] ⚔️ Enemy Attack triggered!");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // ✅ Move back to original position
        while (Vector3.Distance(transform.position, startPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // ✅ Reset to Idle
        animator.SetTrigger("Idle");
        Debug.Log("[EnemyAnimationController] ✅ Enemy returned to Idle.");
    }
}
