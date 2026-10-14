
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

    [SerializeField] private bool saveInitialRotation = true;

    private bool used;
    private Quaternion initialRotation;

    public string Clue => clue;
    public bool CanInteract => !singleUse || !used;

    public bool FacePlayerOnInteract => facePlayerOnInteract;

    private void Awake()
    {
        initialRotation = transform.rotation;
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
                $"[{nameof(InteractionTrigger)}] Player dengan tag 'Player' tidak ditemukan.",
                this
            );
            return;
        }

        Vector2 direction = player.transform.position - transform.position;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }


    public void ReturnToInitialRotation()
    {
        if (!saveInitialRotation) return;

        transform.rotation = initialRotation;
    }

    public void SaveCurrentRotationAsInitial()
    {
        initialRotation = transform.rotation;
    }
}
