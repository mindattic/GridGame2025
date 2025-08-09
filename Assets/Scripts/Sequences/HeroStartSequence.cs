// --- File: Assets/Scripts/Events/Sequences/HeroStartSequence.cs ---
using Assets.Helper;
using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Performs start-of-turn logic for the hero team.
    /// Refills the hero action timer and makes sure UI is in the right mode.
    /// </summary>
    public class HeroStartSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            // Only run during hero turns
            if (!g.TurnManager.isHeroTurn)
                yield break;

            // Small pacing
            yield return Wait.None();

            // Put input back into hero mode and refill the turn timer UI
            g.InputManager.inputMode = InputMode.Player;
            g.TimerBar2D.Refill();   // resets fill to full and timeRemaining to max

            // If you restore AP on hero start, do it here
            // foreach (var hero in g.Actors.Heroes) hero.RestoreAP();

            yield break;
        }
    }
}
