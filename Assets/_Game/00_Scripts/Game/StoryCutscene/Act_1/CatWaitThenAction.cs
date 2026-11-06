
using System.Collections;
using UnityEngine;

public class CatWaitThenAction : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator catAnimator;
    [SerializeField] private Animator potAnimator;

    [Header("Cat Animations")]
    [SerializeField] private AnimationClip catJoinPotAnimation;
    [SerializeField] private AnimationClip catIdleAnimation;
    [SerializeField] private AnimationClip catActionAnimation;

    [Header("Pot Animations")]
    [SerializeField] private AnimationClip potIdleAnimation;
    [SerializeField] private AnimationClip breakPotAnimation;

    [Header("Animation Speed")]
    [SerializeField] private float catJoinSpeed = 1f;
    [SerializeField] private float catIdleSpeed = 1f;
    [SerializeField] private float catActionSpeed = 1f;

    [SerializeField] private float potIdleSpeed = 1f;
    [SerializeField] private float breakPotSpeed = 1f;

    [Header("Wait Settings")]
    [SerializeField] private float waitDuration = 2f;

    [Header("Options")]
    [SerializeField] private bool playJoinOnStart = true;
    [SerializeField] private bool useUnscaledTime = false;

    private bool isJoinPlaying;
    private bool isBreakPlaying;

    public bool IsJoinPlaying => isJoinPlaying;
    public bool IsBreakPlaying => isBreakPlaying;

    private void Awake()
    {
        if (catAnimator == null)
            catAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (playJoinOnStart)
            PlayJoinPotSequence();
    }

    // ==========================================
    // SEQUENCE 1: JOIN POT + IDLE
    // ==========================================

    public void PlayJoinPotSequence()
    {
        if (isJoinPlaying || isBreakPlaying) return;

        if (catAnimator == null ||
            potAnimator == null ||
            catJoinPotAnimation == null ||
            catIdleAnimation == null ||
            potIdleAnimation == null)
        {
            Debug.LogWarning("Join Pot Sequence: Missing references.", this);
            return;
        }

        StartCoroutine(JoinPotSequence());
    }

    private IEnumerator JoinPotSequence()
    {
        isJoinPlaying = true;

        // 1. CAT JOIN POT
        catAnimator.speed = catJoinSpeed;
        catAnimator.Play(catJoinPotAnimation.name, 0, 0f);

        while (true)
        {
            yield return null;

            AnimatorStateInfo stateInfo =
                catAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(catJoinPotAnimation.name) &&
                stateInfo.normalizedTime >= 1f)
                break;
        }

        // 2. CAT IDLE + POT IDLE
        catAnimator.speed = catIdleSpeed;
        potAnimator.speed = potIdleSpeed;

        catAnimator.Play(catIdleAnimation.name, 0, 0f);
        potAnimator.Play(potIdleAnimation.name, 0, 0f);

        isJoinPlaying = false;
    }

    // ==========================================
    // SEQUENCE 2: CAT ACTION + BREAK POT
    // ==========================================

    public void PlayBreakPotSequence()
    {
        if (isJoinPlaying || isBreakPlaying) return;

        if (catAnimator == null ||
            potAnimator == null ||
            catActionAnimation == null ||
            breakPotAnimation == null)
        {
            Debug.LogWarning("Break Pot Sequence: Missing references.", this);
            return;
        }

        StartCoroutine(BreakPotSequence());
    }

    private IEnumerator BreakPotSequence()
    {
        isBreakPlaying = true;

        // CAT ACTION + BREAK POT SIMULTANEOUSLY
        catAnimator.speed = catActionSpeed;
        potAnimator.speed = breakPotSpeed;

        catAnimator.Play(catActionAnimation.name, 0, 0f);
        potAnimator.Play(breakPotAnimation.name, 0, 0f);

        // Wait until cat action finishes
        while (true)
        {
            yield return null;

            AnimatorStateInfo stateInfo =
                catAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(catActionAnimation.name) &&
                stateInfo.normalizedTime >= 1f)
                break;
        }

        catAnimator.speed = 1f;
        potAnimator.speed = 1f;

        isBreakPlaying = false;
    }
}
