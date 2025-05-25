using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public enum SortingScenario
{
    None,
    SelectedHeroDrag,

}


public class SortingManager : MonoBehaviour
{
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected DottedLineManager dottedLineManager => GameManager.instance.dottedLineManager;
    protected bool hasSelectedHero => selectedHero != null;
    protected ActorInstance selectedHero => GameManager.instance.selectedHero;
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;


    private const int Min = 0;
    private const int Default = 50;
    private const int BoardOverlay = 100;
    private const int Opponent = 120;
    private const int Supporter = 140;
    private const int Attacker = 150;
    private const int AttackLine = 200;
    private const int Moving = 900;
    private const int Max = 999;



    public void Set(SortingScenario scenario)
    {
        switch (scenario)
        {
            case SortingScenario.SelectedHeroDrag:

                if (!hasSelectedHero) return;
                selectedHero.transform.GetChild("Front").GetComponent<SortingGroup>().sortingLayerName = "Selected";
                Debug.Log("Changed to layer: " + selectedHero.sortingGroup.sortingLayerName);
                break;
        }
    }





    //// Called when the selected hero is being dragged
    //public void OnSelectedHeroDrag()
    //{
    //    if (!hasSelectedHero) return;

    //    // Set the selected hero's sorting order to the maximum
    //    selectedHero.sortingOrder = Max;

    //    // Reset sorting order for all other actors
    //    foreach (var actor in actors.Where(a => a != selectedHero))
    //    {
    //        actor.sortingOrder = Default;
    //    }
    //}

    //// Called when a pincer attack starts
    //public void OnPincerAttackStart(PincerAttackParticipants participants)
    //{

    //    // Iterate over each valid pair and assign the appropriate sorting order.
    //    foreach (var pair in participants.pair)
    //    {
    //        // Assign both attackers to the attacker sorting order.
    //        pair.attacker1.sortingOrder = Attacker;
    //        pair.attacker2.sortingOrder = Attacker;

    //        // Assign each opponent between the attackers to the opponent sorting order.
    //        foreach (var opp in pair.opponents)
    //            opp.sortingOrder = Opponent;

    //        // Assign all supporters for attacker1 and attacker2 to the supporter sorting order.
    //        foreach (var s in pair.supporters1)
    //            s.sortingOrder = Supporter;
    //        foreach (var s in pair.supporters2)
    //            s.sortingOrder = Supporter;
    //    }

    //}

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