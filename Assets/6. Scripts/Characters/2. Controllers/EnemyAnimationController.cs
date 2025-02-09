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
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>(); // Auto-find Animator in children
        }

        if (animator == null)
        {
            Debug.LogError($"[EnemyAnimationController] ❌ Animator not found on {gameObject.name} or its children!");
        }

        originalPosition = transform.position; // Store initial position
    }

    /// <summary>
    /// ✅ Moves the enemy toward a target.
    /// </summary>
    public IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        if (animator != null)
        {
            animator.SetTrigger("Dash");  // ✅ Trigger Dash animation
        }

        while (Vector3.Distance(transform.position, targetPosition) > 1.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// ✅ Plays the attack sequence: Dash → Attack → Return.
    /// </summary>
    public IEnumerator PlayAttackSequence(Vector3 targetPosition)
    {
        if (animator == null)
        {
            Debug.LogError("[EnemyAnimationController] ❌ No Animator found!");
            yield break;
        }

        Vector3 startPosition = transform.position;

        // ✅ Move toward the enemy & play dash animation
        yield return StartCoroutine(MoveToTarget(targetPosition));

        // ✅ Attack once enemy reaches target
        animator.SetTrigger("AttackStrike");  
        Debug.Log("[EnemyAnimationController] ⚔️ Attack triggered!");

        // ✅ Wait for the attack animation to complete
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // ✅ Move back to original position
        yield return StartCoroutine(MoveToTarget(startPosition));

        // ✅ Set to Idle once back at original position
        animator.SetTrigger("Idle");
    }
}
