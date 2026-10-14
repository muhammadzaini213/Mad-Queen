
using System.Collections;
using UnityEngine;

public class CatBreakPot : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Animations")]
    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip walkAnimation;

    [Header("Movement")]
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float walkSpeed = 2f;

    [Header("Options")]
    [SerializeField] private bool useUnscaledTime = false;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void PlayCutscene()
    {
        if (isPlaying) return;

        if (animator == null)
        {
            return;
        }

        if (targetPosition == null)
        {
            return;
        }

        if (idleAnimation == null || walkAnimation == null)
        {
            return;
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        isPlaying = true;

        animator.Play(walkAnimation.name, 0, 0f);

        while (Vector2.Distance(transform.position, targetPosition.position) > 0.02f)
        {
            Vector2 currentPosition = transform.position;
            Vector2 destination = targetPosition.position;

            transform.position = Vector2.MoveTowards(
                currentPosition,
                destination,
                walkSpeed * (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime)
            );

            yield return null;
        }

        transform.position = new Vector3(
            targetPosition.position.x,
            targetPosition.position.y,
            transform.position.z
        );

        animator.Play(idleAnimation.name, 0, 0f);

        isPlaying = false;
    }
}
