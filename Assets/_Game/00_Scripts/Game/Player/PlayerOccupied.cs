
using UnityEngine;
using Slafurry.Core.Abstract;
using System.Collections;

public class PlayerOccupied : LocalSingleton<PlayerOccupied>
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInteract playerInteract;

    public bool IsOccupied { get; private set; }

    public void Occupy()
    {
        IsOccupied = true;
        RefreshState();
    }

    public void Vacate()
    {
        IsOccupied = false;
        RefreshState();
    }

    private void RefreshState()
    {
        if (playerMovement != null)
            playerMovement.enabled = !IsOccupied;

        if (playerInteract != null)
            playerInteract.enabled = !IsOccupied;
    }

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();

        IsOccupied = false;
        RefreshState();
    }

    public override IEnumerator Initialize()
    {
        yield return null;
    }

    public override void PostInitialize()
    {
    }
}
