using UnityEngine;
using System.Collections;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float attackDistance = 1.5f;
    [SerializeField] protected float attackDuration = 1f;

    protected Vector3 originalPosition;
    protected bool isAnimating = false;

    protected virtual void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        originalPosition = transform.position;
    }

    public virtual IEnumerator MoveToTarget(Vector3 targetPosition, bool returnToStart = false)
    {
        Vector3 startPos = transform.position;
        Vector3 destination = targetPosition;

        // Start movement animation
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
        
        // Ensure exact position and stop movement animation
        transform.position = destination;
        animator.SetBool("IsMoving", false);
    }

    public virtual IEnumerator PlayAttackSequence(Vector3 targetPosition)
    {
        if (isAnimating) yield break;
        isAnimating = true;

        // Move to attack position
        Vector3 attackPosition = Vector3.MoveTowards(targetPosition, transform.position, attackDistance);
        yield return StartCoroutine(MoveToTarget(attackPosition));

        // Wait a frame to ensure movement is complete
        yield return null;

        // Play attack animation
        animator.SetTrigger("AttackStrike");
        
        // Wait for attack animation
        float attackStateLength = GetAnimationLength("AttackStrike");
        yield return new WaitForSeconds(attackStateLength);

        // Return to starting position
        yield return StartCoroutine(MoveToTarget(originalPosition, true));

        // Reset to idle
        animator.SetTrigger("Idle");

        isAnimating = false;
    }

    protected float GetAnimationLength(string stateName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    protected virtual void OnValidate()
    {
        if (moveSpeed <= 0) moveSpeed = 3f;
        if (attackDistance <= 0) attackDistance = 1.5f;
        if (attackDuration <= 0) attackDuration = 1f;
    }
}
