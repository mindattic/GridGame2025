using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Performs any start-of-turn animation or logic for the hero team.
    /// </summary>
    public class HeroStartSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            // Only run during hero turns
            if (!g.TurnManager.isHeroTurn)
                yield break;

            // Wait for any animations/effects to finish
            yield return null;

            // Restore AP for all hero-controlled actors
            //foreach (var hero in g.Actors.Heroes)
            //{
            //    hero.RestoreAP();
            //}

            yield break;
        }
    }
}
