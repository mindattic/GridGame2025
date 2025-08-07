using System;
using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

/// <summary>
/// Types of sorting requests.
/// </summary>
public enum SortEventType
{
    Default,
    Focus,
    Drag,
    LocationChanged,
    Drop,
    ActorMoving,
    Overlap,
    PincerAttack,
    Bump
}

/// <summary>
/// Context for a sorting request. Carries all the data needed by actors to decide their sorting.
/// </summary>
public class SortEvent
{
    public SortEventType Type;
    public ActorInstance Initiator;
    public ActorInstance Target;
    public Vector2Int? NewLocation;
    public PincerAttackParticipants Participants;
}

/// <summary>
/// Manages global sorting requests by raising events that actors handle individually.
/// </summary>
public class SortingManager : MonoBehaviour
{
   

    /// <summary>
    /// Global event actors subscribe to in order to update their sorting.
    /// </summary>
    public static event Action<SortEvent> OnSortRequested;

    /// <summary>
    /// Invokes the sorting event with the given context.
    /// </summary>
    private void Invoke(SortEvent e)
    {
        OnSortRequested?.Invoke(e);
    }

    public void OnActorFocus()
    {
        if (!g.Actors.HasFocusedActor) return;
        Invoke(new SortEvent { 
            Type = SortEventType.Focus, 
            Initiator = g.Actors.FocusedActor
        });
    }

    public void OnSelectedHeroDrag()
    {
        if (!g.Actors.HasSelectedHero) return;
        Invoke(new SortEvent
        {
            Type = SortEventType.Drag,
            Initiator = g.Actors.SelectedHero
        });
    }

    public void OnSelectedHeroLocationChanged(Vector2Int newLocation)
    {
        if (!g.Actors.HasSelectedHero) return;
        Invoke(new SortEvent
        {
            Type = SortEventType.LocationChanged,
            Initiator = g.Actors.SelectedHero,
            NewLocation = newLocation
        });
    }

    public void OnSelectedHeroDrop()
    {
        if (!g.Actors.HasSelectedHero) return;
        Invoke(new SortEvent
        {
            Type = SortEventType.Drop,
            Initiator = g.Actors.SelectedHero
        });
    }

    public void OnActorMoving(ActorInstance actor)
    {
        Invoke(new SortEvent
        {
            Type = SortEventType.ActorMoving,
            Initiator = actor
        });
    }

    public void OnActorOverlap(ActorInstance initiator, ActorInstance target)
    {
        Invoke(new SortEvent
        {
            Type = SortEventType.Overlap,
            Initiator = initiator,
            Target = target
        });
    }

    public void OnPincerAttack(PincerAttackParticipants participants)
    {
        Invoke(new SortEvent
        {
            Type = SortEventType.PincerAttack,
            Participants = participants
        });
    }

    public void OnBump(ActorInstance initiator, ActorInstance target)
    {
        Invoke(new SortEvent
        {
            Type = SortEventType.Bump,
            Initiator = initiator,
            Target = target
        });
    }

    // These two rely on existing direct layering logic:
    public void OnSupportLineSpawn(SupportLineInstance supportLine)
    {
        var isAbove = supportLine.supporter.sortingGroup.sortingLayerName == SortingHelper.Layer.ActorAbove;
        supportLine.SetSorting(isAbove ? SortingHelper.Layer.SupportLineAbove : SortingHelper.Layer.SupportLineBelow);
    }

    public void OnPortraitPopIn(Portrait3DInstance portrait)
    {
        portrait.SetSorting(SortingHelper.Layer.PortraitPopIn, SortingHelper.Order.Max);
    }
}
