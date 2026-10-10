using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractionTrigger : MonoBehaviour
{
    [SerializeField] private string clue = "Interact";
    [SerializeField] private bool singleUse;
    [SerializeField] private UnityEvent onInteract;

    private bool used;

    public string Clue => clue;
    public bool CanInteract => !singleUse || !used;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Interact()
    {
        if (!CanInteract) return;

        used = true;
        onInteract?.Invoke();
    }
}