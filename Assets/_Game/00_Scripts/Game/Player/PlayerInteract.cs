using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private InteractionHUD hud;
    [SerializeField] private Key interactKey = Key.E;

    private readonly List<InteractionTrigger> nearby = new List<InteractionTrigger>();
    private InteractionTrigger current;

    private void Update()
    {
        if (DialogHUD.IsBusy)
        {
            hud.Hide();
            return;
        }

        current = GetClosest();

        if (current == null)
        {
            hud.Hide();
            return;
        }

        hud.Show(current.Clue);

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard[interactKey].wasPressedThisFrame)
            current.Interact();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractionTrigger trigger) && !nearby.Contains(trigger))
            nearby.Add(trigger);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractionTrigger trigger))
            nearby.Remove(trigger);
    }

    private InteractionTrigger GetClosest()
    {
        nearby.RemoveAll(t => t == null);

        InteractionTrigger closest = null;
        float closestDistance = float.MaxValue;

        foreach (InteractionTrigger trigger in nearby)
        {
            if (!trigger.isActiveAndEnabled || !trigger.CanInteract) continue;

            float distance = (trigger.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = trigger;
            }
        }

        return closest;
    }
}