using UnityEngine;
using System.Collections;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 3f;
    private Vector3 originalPosition;

    public Vector3 OriginalPosition => originalPosition;

    private void Awake()
    {
        // ✅ Automatically find the Animator even if it's on a child object
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError($"[CharacterAnimationController] ❌ Animator not found on {gameObject.name} or its children!");
        }

        originalPosition = transform.position; // Store initial position
    }

    /// <summary>
    /// ✅ Moves the character to a target position.
    /// </summary>
    public IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 10.0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// ✅ Plays the attack sequence: Dash → Attack → Return.
    /// </summary>
    public IEnumerator PlayAttackSequence(Vector3 enemyPosition)
    {
        if (animator == null)
        {
            Debug.LogError("[CharacterAnimationController] ❌ No Animator found!");
            yield break;
        }

        Vector3 startPosition = transform.position;

        // ✅ Trigger Dash animation
        animator.SetTrigger("Dash");

        // ✅ Move toward the enemy, but check distance dynamically
        while (Vector3.Distance(transform.position, enemyPosition) > 1.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, enemyPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // ✅ Immediately trigger the attack once the character reaches the enemy
        animator.SetTrigger("AttackStrike");
        Debug.Log("[CharacterAnimationController] ⚔️ Attack triggered as soon as character reached enemy!");

        // ✅ Wait until the attack animation completes
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // ✅ Move back to the original position
        while (Vector3.Distance(transform.position, startPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // ✅ Reset to Idle
        animator.SetTrigger("Idle");
        Debug.Log("[CharacterAnimationController] ✅ Returned to Idle.");
    }


}
