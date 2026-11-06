using UnityEngine;
using UnityEngine.Events;

public class AreaTrigger : BaseTrigger
{
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;
    [SerializeField] private UnityEvent onTriggerStay;
    [SerializeField] private string triggerTag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger Enter: {other.name}");
        if (!CanTrigger()) return;
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;

        Debug.Log($"Trigger Enter: {other.name} - Invoking");
        onTriggerEnter?.Invoke();
        AddTriggerCount();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Trigger Exit: {other.name}");
        if (!CanTrigger()) return;
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;

        Debug.Log($"Trigger Exit: {other.name} - Invoking");
        onTriggerExit?.Invoke();
        AddTriggerCount();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"Trigger Stay: {other.name}");
        if (!CanTrigger()) return;
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;

        Debug.Log($"Trigger Stay: {other.name} - Invoking");
        onTriggerStay?.Invoke();
        AddTriggerCount();
    }

}
