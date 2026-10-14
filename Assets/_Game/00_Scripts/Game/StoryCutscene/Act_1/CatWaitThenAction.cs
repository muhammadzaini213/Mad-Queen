
using System.Collections;
using UnityEngine;

public class CatWaitThenAction : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Animations")]
    [SerializeField] private AnimationClip potIdleAnimation;
    [SerializeField] private AnimationClip breakPotAnimation;

    [Header("Wait Settings")]
    [SerializeField] private float waitDuration = 2f;

    [Header("Options")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool useUnscaledTime = false;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (playOnStart)
            PlaySequence();
    }

    public void PlaySequence()
    {
        if (isPlaying) return;

        if (animator == null ||
            potIdleAnimation == null ||
            breakPotAnimation == null)
        {
            Debug.LogWarning(
                "CatWaitThenAction: Animator atau Animation Clip belum dipasang.",
                this
            );
            return;
        }

        StartCoroutine(WaitThenAct());
    }

    private IEnumerator WaitThenAct()
    {
        isPlaying = true;

        animator.Play(potIdleAnimation.name, 0, 0f);

        float elapsed = 0f;

        while (elapsed < waitDuration)
        {
            elapsed += useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            yield return null;
        }

        animator.Play(breakPotAnimation.name, 0, 0f);

        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        while (stateInfo.IsName(breakPotAnimation.name) &&
               stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        isPlaying = false;
    }
}
