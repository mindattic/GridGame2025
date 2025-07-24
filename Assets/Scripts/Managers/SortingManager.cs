using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

public class SortingManager : MonoBehaviour
{


    //Sorting layers
    public static class SortingLayer
    {
        public const string Default = "Default";
        public const string Board = "BoardManager";
        public const string DottedLine = "DottedLine";
        public const string SupportLineBelow = "SupportLineBelow";
        public const string ActorBelow = "ActorBelow";
        public const string BoardOverlay = "BoardOverlay";
        public const string SupportLineAbove = "SupportLineAbove";
        public const string ActorAbove = "ActorAbove";
        public const string VFX = "VfxManager";
        public const string Coin = "Coin";
        public const string DamageText = "DamageTextManager";
        public const string PortraitPopIn = "PortraitPopIn";
        public const string Portrait = "PortraitManager";
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
        if (!g.Actors.HasFocusedActor) return;
        g.Actors.All.ForEach(x => x.SetSorting(SortingLayer.ActorBelow));
        g.Actors.FocusedActor.SetSorting(SortingLayer.ActorAbove);
    }

    public void OnSelectedHeroDrag()
    {
        if (!g.Actors.HasSelectedHero) return;
        g.Actors.All.ForEach(x => x.SetSorting(SortingLayer.ActorBelow));
        g.Actors.SelectedHero.SetSorting(SortingLayer.ActorAbove);
    }


    public void OnSelectedHeroLocationChanged(Vector2Int newLocation)
    {
        foreach (var actor in g.Actors.All)
        {

            if (actor == g.Actors.SelectedHero)
            {
                actor.SetSorting(SortingLayer.ActorAbove, SortingOrder.Max);
            }
            else
            {
                actor.SetSorting(SortingLayer.ActorBelow, SortingOrder.Min);
            }
        }
    }




    public void OnSelectedHeroDrop()
    {
        g.Actors.All.ForEach(x => x.SetSorting(SortingLayer.ActorBelow));
    }

    public void OnActorMoving(ActorInstance actor)
    {
        g.Actors.All.ForEach(x => x.SetSorting(SortingLayer.ActorBelow));
        actor.SetSorting(SortingLayer.ActorAbove, 0);
    }


    public void OnActorOverlap(ActorInstance initiator, ActorInstance target)
    {
        // Bring initiator to front
        initiator.SetSorting(SortingLayer.ActorAbove, SortingOrder.Max);

        // Push target below
        target.SetSorting(SortingLayer.ActorBelow, SortingOrder.Min);
    }


    public void OnPincerAttackStart(PincerAttackParticipants participants)
    {

        g.Actors.All.ForEach(x => x.SetSorting(SortingLayer.ActorBelow));

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

    public void OnSupportLineSpawn(SupportLineInstance supportLine)
    {
        var isAbove = supportLine.supporter.sortingGroup.sortingLayerName == SortingLayer.ActorAbove;
        supportLine.SetSorting(isAbove ? SortingLayer.SupportLineAbove : SortingLayer.SupportLineBelow);
    }

    public void OnPortraitPopIn(PortraitInstance portrait)
    {
        portrait.SetSorting(SortingLayer.PortraitPopIn, SortingOrder.Max);
    }

    //// Called when a pincer attackResult ends
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