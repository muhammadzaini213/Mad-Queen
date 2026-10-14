
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private InteractionHUD hud;
    [SerializeField] private Key interactKey = Key.E;

    [Header("Highlight")]
    [SerializeField] private Material highlightMaterial;

    [Header("Dialog")]
    [SerializeField] private DialogHUD[] dialogHUDs;

    private readonly List<InteractionTrigger> nearby =
        new List<InteractionTrigger>();

    private readonly Dictionary<SpriteRenderer, Material> originalMaterials =
        new Dictionary<SpriteRenderer, Material>();

    private InteractionTrigger current;
    private InteractionTrigger highlighted;

    private void Awake()
    {
        // Jika tidak diisi melalui Inspector, cari DialogHUD di scene.
        if (dialogHUDs == null || dialogHUDs.Length == 0)
        {
            dialogHUDs = FindObjectsByType<DialogHUD>(
                FindObjectsSortMode.None
            );
        }
    }

    private void Update()
    {
        if (IsDialogBusy())
        {
            current = null;

            if (hud != null)
                hud.Hide();

            ClearHighlight();
            return;
        }

        current = GetClosest();

        if (current == null)
        {
            if (hud != null)
                hud.Hide();

            ClearHighlight();
            return;
        }

        if (hud != null)
            hud.Show(current.Clue);

        SetHighlight(current);

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null &&
            keyboard[interactKey].wasPressedThisFrame)
        {
            current.Interact();
        }
    }

    private bool IsDialogBusy()
    {
        // Periksa semua HUD dialog, tanpa singleton.
        if (dialogHUDs == null)
            return false;

        foreach (DialogHUD dialog in dialogHUDs)
        {
            if (dialog != null && dialog.IsBusy)
                return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractionTrigger trigger) &&
            !nearby.Contains(trigger))
        {
            nearby.Add(trigger);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out InteractionTrigger trigger))
            return;

        nearby.Remove(trigger);

        if (highlighted == trigger)
            ClearHighlight();
    }

    private InteractionTrigger GetClosest()
    {
        nearby.RemoveAll(t => t == null);

        InteractionTrigger closest = null;
        float closestDistance = float.MaxValue;

        foreach (InteractionTrigger trigger in nearby)
        {
            if (trigger == null ||
                !trigger.isActiveAndEnabled ||
                !trigger.CanInteract)
            {
                continue;
            }

            float distance = (
                trigger.transform.position - transform.position
            ).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = trigger;
            }
        }

        return closest;
    }

    private void SetHighlight(InteractionTrigger target)
    {
        if (highlighted == target)
            return;

        // Pulihkan material target sebelumnya.
        ClearHighlight();

        if (target == null || highlightMaterial == null)
            return;

        highlighted = target;

        SpriteRenderer[] renderers =
            target.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null)
                continue;

            // Simpan material asli satu kali.
            if (!originalMaterials.ContainsKey(sr))
            {
                originalMaterials.Add(sr, sr.sharedMaterial);
            }

            // Jangan override tekstur sprite secara manual.
            sr.sharedMaterial = highlightMaterial;
        }
    }

    private void ClearHighlight()
    {
        foreach (KeyValuePair<SpriteRenderer, Material> entry
                 in originalMaterials)
        {
            if (entry.Key != null)
            {
                entry.Key.sharedMaterial = entry.Value;
            }
        }

        originalMaterials.Clear();
        highlighted = null;
    }

    private void OnDisable()
    {
        if (hud != null)
            hud.Hide();

        ClearHighlight();
    }
}
