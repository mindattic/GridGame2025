using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;




public class SortingManager : MonoBehaviour
{
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected DottedLineManager dottedLineManager => GameManager.instance.dottedLineManager;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;

    protected ActorInstance selectedHero => GameManager.instance.selectedHero;

    protected bool hasFocusedActor => focusedActor != null;
    protected bool hasSelectedHero => selectedHero != null;

    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;


    //Sorting layers
    public static class SortingLayer
    {
        public const string Default = "Default";
        public const string Board = "Board";
        public const string DottedLine = "DottedLine";
        public const string ActorBelow = "ActorBelow";
        public const string BoardOverlay = "BoardOverlay";
        public const string SupportLine = "SupportLine";
        public const string ActorAbove = "ActorAbove";
        public const string VFX = "VFX";
        public const string Coin = "Coin";
        public const string DamageText = "DamageText";
    }


    //Sorting orders
    private static class SortingOrder
    {
        public const int Min = 0;
        public const int Opponent = 100;
        public const int Supporter = 200;
        public const int Attacker = 300;
        public const int AttackLine = 400;
        public const int Max = 999;
    }


    public void OnActorFocus()
    {
        if (!hasFocusedActor) return;
        actors.ForEach(x => x.SetSorting(SortingLayer.ActorBelow));
        focusedActor.SetSorting(SortingLayer.ActorAbove);
    }

    public void OnSelectedHeroDrag()
    {
        if (!hasSelectedHero) return;
        actors.ForEach(x => x.SetSorting(SortingLayer.ActorBelow));
        selectedHero.SetSorting(SortingLayer.ActorAbove);
    }


    //public void OnSelectedHeroLocationChanged(Vector2Int newLocation)
    //{
    //    foreach (var actor in actors)
    //    {

    //        if (actor == selectedHero)
    //        {
    //            actor.SetSorting(SortingLayer.ActorAbove, GetDepthOrder(actor.transform.position));
    //        }
    //        else
    //        {
    //            actor.SetSorting(SortingLayer.ActorBelow, GetDepthOrder(actor.transform.position));
    //        }
    //    }
    //}




    public void OnSelectedHeroDrop()
    {
        actors.ForEach(x => x.SetSorting(SortingLayer.ActorBelow));
    }

    public void OnActorOverlap(ActorInstance initiator, ActorInstance target)
    {
        // Bring initiator to front
        initiator.SetSorting(SortingLayer.ActorAbove, GetDepthOrder(initiator.transform.position));

        // Push target below
        target.SetSorting(SortingLayer.ActorBelow, GetDepthOrder(target.transform.position));
    }

    private int GetDepthOrder(Vector3 position)
    {
        // Y-axis-based depth sorting (lower Y = higher priority)
        return Mathf.RoundToInt(-position.y * 100);
    }

    public void OnPincerAttackStart(PincerAttackParticipants participants)
    {

        actors.ForEach(x => x.SetSorting(SortingLayer.ActorBelow));

        // Iterate over each valid pair and assign the appropriate sorting order.
        foreach (var pair in participants.pair)
        {
            // Assign both attackers to the attacker sorting order.
            pair.attacker1.SetSorting(SortingLayer.ActorAbove, SortingOrder.Attacker);
            pair.attacker2.SetSorting(SortingLayer.ActorAbove, SortingOrder.Attacker);

            // Assign each opponent between the attackers to the opponent sorting order.
            foreach (var x in pair.opponents)
                x.SetSorting(SortingLayer.ActorAbove, SortingOrder.Opponent);

            // Assign all supporters for attacker1 and attacker2 to the supporter sorting order.
            foreach (var x in pair.supporters1)
                x.SetSorting(SortingLayer.ActorAbove, SortingOrder.Supporter);
            foreach (var x in pair.supporters2)
                x.SetSorting(SortingLayer.ActorAbove, SortingOrder.Supporter);
        }

    }

    //// Called when a pincer attack ends
    //public void OnPincerAttackEnd(List<ActorInstance> involvedActors)
    //{
    //    foreach (var actor in involvedActors)
    //    {
    //        actor.sortingOrder = Default;
    //    }
    //}

    //// Called when the turn restarts
    //public void OnTurnRestart()
    //{
    //    // Reset all actors to their default sorting order
    //    foreach (var actor in actors)
    //    {
    //        actor.sortingOrder = Default;
    //    }

    //    // Ensure board overlay and other elements are properly layered
    //    //boardOverlay.sortingOrder = BoardOverlay;
    //}
}