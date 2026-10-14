
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CatGlitch : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    [Header("Run Animation")]
    [SerializeField] private AnimationClip runAnimation;

    [Header("Movement")]
    [SerializeField] private Transform firstTarget;
    [SerializeField] private Transform secondTarget;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Glitch Settings")]
    [SerializeField] private float glitchDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Events")]
    [SerializeField] private UnityEvent onSequenceEnds;

    [Header("Options")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool useUnscaledTime = false;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderers == null || spriteRenderers.Length == 0)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void Start()
    {
        if (playOnStart)
            PlayGlitch();
    }

    public void PlayGlitch()
    {
        if (isPlaying) return;

        if (animator == null ||
            runAnimation == null ||
            firstTarget == null ||
            secondTarget == null)
        {
            Debug.LogWarning(
                this
            );
            return;
        }

        StartCoroutine(GlitchSequence());
    }

    private IEnumerator GlitchSequence()
    {
        isPlaying = true;

        SetVisible(true);
        animator.speed = 1f;
        animator.Play(runAnimation.name, 0, 0f);

        yield return MoveTo(firstTarget.position);

        animator.speed = 0f;

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

        SetVisible(true);

        animator.speed = 1f;

        yield return MoveTo(secondTarget.position);

        SetVisible(true);
        animator.speed = 1f;
        isPlaying = false;

        onSequenceEnds?.Invoke();
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        while (Vector2.Distance(transform.position, destination) > 0.02f)
        {
            float delta = useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            Vector3 target = new Vector3(
                destination.x,
                destination.y,
                transform.position.z
            );

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * delta
            );

            yield return null;
        }

        // Pastikan posisi tepat pada target.
        transform.position = new Vector3(
            destination.x,
            destination.y,
            transform.position.z
        );
    }

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

    private void SetVisible(bool visible)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr != null)
                sr.enabled = visible;
        }
    }

    public void StopGlitch()
    {
        StopAllCoroutines();

        if (animator != null)
            animator.speed = 1f;

        SetVisible(true);
        isPlaying = false;
    }
}
