using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractionTrigger : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private string clue = "Interact";
    [SerializeField] private bool singleUse;
    [SerializeField] private UnityEvent onInteract;

    [Header("Face Player")]
    [SerializeField] private bool facePlayerOnInteract = false;

    [Tooltip("Uncheck if the original sprite faces left.")]
    [SerializeField] private bool spriteFacesRight = true;

    [Tooltip("SpriteRenderer to flip.")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool used;

    public string Clue => clue;
    public bool CanInteract => !singleUse || !used;
    public bool FacePlayerOnInteract => facePlayerOnInteract;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogWarning(
                $"[{nameof(InteractionTrigger)}] SpriteRenderer not found on {name}.",
                this
            );
        }
    }

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Interact()
    {
        if (!CanInteract) return;

        used = true;

        if (facePlayerOnInteract)
        {
            FacePlayer();
        }

        onInteract?.Invoke();
    }

    private void FacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning(
                $"[{nameof(InteractionTrigger)}] Player with tag 'Player' not found.",
                this
            );
            return;
        }

        FlipToward(player.transform);
    }

    public void FlipToward(Transform target)
    {
        if (target == null || spriteRenderer == null)
            return;

        FlipToward((Vector2)target.position);
    }

    public void FlipToward(Vector2 worldPosition)
    {
        if (spriteRenderer == null)
            return;

        float directionX =
            worldPosition.x - spriteRenderer.transform.position.x;

        if (Mathf.Abs(directionX) < 0.0001f)
            return;

        bool targetIsRight = directionX > 0f;

        spriteRenderer.flipX = spriteFacesRight
            ? !targetIsRight
            : targetIsRight;
    }
}