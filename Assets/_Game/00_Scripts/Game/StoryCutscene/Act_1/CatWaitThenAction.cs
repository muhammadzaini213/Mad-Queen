
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CatWaitThenAction : MonoBehaviour
{
    // ==========================================
    // COMPONENTS
    // ==========================================

    [Header("Components")]
    [SerializeField] private Animator catAnimator;
    [SerializeField] private Animator potAnimator;
    [SerializeField] private SpriteRenderer[] spriteRenderers;


    // ==========================================
    // CAT ANIMATIONS
    // ==========================================

    [Header("Cat Animations")]
    [SerializeField] private AnimationClip catJoinPotAnimation;
    [SerializeField] private AnimationClip catIdleAnimation;
    [SerializeField] private AnimationClip catActionAnimation;


    // ==========================================
    // POT ANIMATIONS
    // ==========================================

    [Header("Pot Animations")]
    [SerializeField] private AnimationClip potIdleAnimation;
    [SerializeField] private AnimationClip breakPotAnimation;


    // ==========================================
    // ANIMATION SPEED
    // ==========================================

    [Header("Animation Speed - Cat")]
    [SerializeField] private float catJoinSpeed = 1f;
    [SerializeField] private float catIdleSpeed = 1f;
    [SerializeField] private float catActionSpeed = 1f;

    [Header("Animation Speed - Pot")]
    [SerializeField] private float potIdleSpeed = 1f;
    [SerializeField] private float breakPotSpeed = 1f;


    // ==========================================
    // GLITCH SETTINGS
    // ==========================================

    [Header("Glitch Settings")]
    [SerializeField] private bool enableGlitch = true;

    [Tooltip("Glitch starts when Pot Break reaches this normalized time.")]
    [Range(0f, 1f)]
    [SerializeField] private float glitchStartNormalizedTime = 0.9f;

    [Tooltip("How long the glitch lasts.")]
    [SerializeField] private float glitchDuration = 1.5f;

    [Tooltip("Time between visibility changes.")]
    [SerializeField] private float blinkInterval = 0.1f;


    // ==========================================
    // EVENTS
    // ==========================================

    [Header("Events")]
    [SerializeField] private UnityEvent onSequenceEnds;


    // ==========================================
    // OPTIONS
    // ==========================================

    [Header("Options")]
    [SerializeField] private bool playJoinOnStart = true;
    [SerializeField] private bool useUnscaledTime = false;


    // ==========================================
    // INTERNAL
    // ==========================================

    private bool isJoinPlaying;
    private bool isBreakPlaying;

    public bool IsJoinPlaying => isJoinPlaying;
    public bool IsBreakPlaying => isBreakPlaying;


    // ==========================================
    // INITIALIZATION
    // ==========================================

    private void Awake()
    {
        if (catAnimator == null)
            catAnimator = GetComponent<Animator>();

        if (spriteRenderers == null || spriteRenderers.Length == 0)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
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
        if (isJoinPlaying || isBreakPlaying)
            return;

        if (catAnimator == null ||
            potAnimator == null ||
            catJoinPotAnimation == null ||
            catIdleAnimation == null ||
            potIdleAnimation == null)
        {
            Debug.LogWarning(
                "Join Pot Sequence: Missing references.",
                this
            );
            return;
        }

        StartCoroutine(JoinPotSequence());
    }

    private IEnumerator JoinPotSequence()
    {
        isJoinPlaying = true;

        // CAT JOIN POT
        catAnimator.speed = catJoinSpeed;
        catAnimator.Play(catJoinPotAnimation.name, 0, 0f);

        // Wait until animation finishes
        while (true)
        {
            yield return null;

            AnimatorStateInfo stateInfo =
                catAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(catJoinPotAnimation.name) &&
                stateInfo.normalizedTime >= 1f)
            {
                break;
            }
        }

        // CAT IDLE + POT IDLE
        catAnimator.speed = catIdleSpeed;
        potAnimator.speed = potIdleSpeed;

        catAnimator.Play(catIdleAnimation.name, 0, 0f);
        potAnimator.Play(potIdleAnimation.name, 0, 0f);

        isJoinPlaying = false;
    }


    // ==========================================
    // SEQUENCE 2: BREAK POT + GLITCH
    // ==========================================

    public void PlayBreakPotSequence()
    {
        if (isJoinPlaying || isBreakPlaying)
            return;

        if (catAnimator == null ||
            potAnimator == null ||
            catActionAnimation == null ||
            breakPotAnimation == null)
        {
            Debug.LogWarning(
                "Break Pot Sequence: Missing references.",
                this
            );
            return;
        }

        StartCoroutine(BreakPotSequence());
    }

    private IEnumerator BreakPotSequence()
    {
        isBreakPlaying = true;

        SetVisible(true);

        // START BOTH ANIMATIONS
        catAnimator.speed = catActionSpeed;
        potAnimator.speed = breakPotSpeed;

        catAnimator.Play(catActionAnimation.name, 0, 0f);
        potAnimator.Play(breakPotAnimation.name, 0, 0f);

        yield return null;

        // WAIT FOR POT BREAK GLITCH TIMING
        if (enableGlitch)
        {
            while (true)
            {
                yield return null;

                AnimatorStateInfo potState =
                    potAnimator.GetCurrentAnimatorStateInfo(0);

                if (potState.IsName(breakPotAnimation.name) &&
                    potState.normalizedTime >= glitchStartNormalizedTime)
                {
                    break;
                }
            }

            // PAUSE CAT ANIMATION
            catAnimator.speed = 0f;

            // GLITCH BLINK
            float elapsed = 0f;
            bool visible = true;

            while (elapsed < glitchDuration)
            {
                visible = !visible;
                SetVisible(visible);

                float interval = Mathf.Max(0.02f, blinkInterval);

                yield return Wait(interval);
                elapsed += interval;
            }

            // RESTORE VISIBILITY
            SetVisible(true);

            // RESUME CAT ANIMATION
            catAnimator.speed = catActionSpeed;
        }

        // WAIT UNTIL BOTH ANIMATIONS FINISH
        bool catFinished = false;
        bool potFinished = false;

        while (!catFinished || !potFinished)
        {
            yield return null;

            AnimatorStateInfo catState =
                catAnimator.GetCurrentAnimatorStateInfo(0);

            AnimatorStateInfo potState =
                potAnimator.GetCurrentAnimatorStateInfo(0);

            catFinished =
                catState.IsName(catActionAnimation.name) &&
                catState.normalizedTime >= 1f;

            potFinished =
                potState.IsName(breakPotAnimation.name) &&
                potState.normalizedTime >= 1f;
        }

        // RESET
        SetVisible(true);

        catAnimator.speed = 1f;
        potAnimator.speed = 1f;

        isBreakPlaying = false;

        // INVOKE AFTER EVERYTHING FINISHES
        onSequenceEnds?.Invoke();
    }


    // ==========================================
    // GLITCH WAIT
    // ==========================================

    private IEnumerator Wait(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            yield return null;
        }
    }


    // ==========================================
    // VISIBILITY
    // ==========================================

    private void SetVisible(bool visible)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr != null)
                sr.enabled = visible;
        }
    }


    // ==========================================
    // STOP SEQUENCE
    // ==========================================

    public void StopSequence()
    {
        StopAllCoroutines();

        if (catAnimator != null)
            catAnimator.speed = 1f;

        if (potAnimator != null)
            potAnimator.speed = 1f;

        SetVisible(true);

        isJoinPlaying = false;
        isBreakPlaying = false;
    }
}
