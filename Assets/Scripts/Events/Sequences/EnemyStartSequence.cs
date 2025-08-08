using Assets.Scripts.Models;
using System.Collections;
using System.Linq;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Performs any start-of-turn animation or logic for the enemy team.
    /// Does not advance phase or add move sequences—TurnManager handles that.
    /// </summary>
    public class EnemyStartSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            // Only run during enemy turns.
            if (!g.TurnManager.isEnemyTurn)
                yield break;

            // Wait for any animations/effects to finish
            yield return null;

            // Now enqueue the move sequences for ready enemies
            foreach (var enemy in g.Actors.Enemies.Where(x => x.isReady))
            {
                g.SequenceManager.Add(new EnemyMoveSequence(enemy));
            }

            g.TurnManager.SetPhase(TurnPhase.PreAttack);
            yield break;
        }
    }



}
