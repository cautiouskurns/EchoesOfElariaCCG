using UnityEngine;
using System.Collections;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackDuration = 1f;
    
    private Vector3 originalPosition;
    public Vector3 OriginalPosition => originalPosition;
    private bool isAnimating = false;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        originalPosition = transform.position;
        Debug.Log($"[EnemyAnimationController] Original position set to: {originalPosition}");
    }

    public IEnumerator MoveToTarget(Vector3 targetPosition, bool returnToStart = false)
    {
        Vector3 startPos = transform.position;
        Vector3 destination = targetPosition;

        // Start moving
        animator.SetBool("IsMoving", true);
        
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        // Ensure exact position and stop moving
        transform.position = destination;
        animator.SetBool("IsMoving", false);
        
        Debug.Log($"[EnemyAnimationController] Moved to {(returnToStart ? "original position" : "target")}");
    }

    public IEnumerator PlayAttackSequence(Vector3 targetPosition)
    {
        if (isAnimating) yield break;
        isAnimating = true;

        Debug.Log("[EnemyAnimationController] Starting attack sequence");

        // Move to attack position
        Vector3 attackPosition = Vector3.MoveTowards(targetPosition, transform.position, attackDistance);
        yield return StartCoroutine(MoveToTarget(attackPosition));

        // Wait a frame to ensure movement animation is complete
        yield return null;

        // Play attack animation
        animator.SetTrigger("AttackStrike");
        Debug.Log("[EnemyAnimationController] Playing attack animation");
        
        // Wait for attack animation to complete
        float attackStateLength = GetAnimationLength("AttackStrike");
        yield return new WaitForSeconds(attackStateLength);

        // Return to original position
        yield return StartCoroutine(MoveToTarget(originalPosition, true));

        // Wait a frame to ensure movement is complete
        yield return null;

        // Reset to idle
        animator.SetTrigger("Idle");
        Debug.Log("[EnemyAnimationController] Returned to idle state");

        isAnimating = false;
    }

    private float GetAnimationLength(string stateName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    private void OnValidate()
    {
        if (moveSpeed <= 0) moveSpeed = 3f;
        if (attackDistance <= 0) attackDistance = 1.5f;
        if (attackDuration <= 0) attackDuration = 1f;
    }
}
