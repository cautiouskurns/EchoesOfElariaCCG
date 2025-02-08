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
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
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

        // ✅ Set Dash animation and move forward
        animator.SetTrigger("Dash");
        yield return StartCoroutine(MoveToTarget(enemyPosition));

        // ✅ Ensure Dash finishes before triggering Attack
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // ✅ Set Attack animation
        animator.SetTrigger("AttackStrike");

        // ✅ Ensure Attack plays fully before next action
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // ✅ Move back to the original position
        yield return StartCoroutine(MoveToTarget(startPosition));

        // ✅ Ensure transition back to Idle after movement
        animator.SetTrigger("Idle");
    }


}
