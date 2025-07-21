using Assets.Scripts.Models;
using System.Collections;
using g = GameManagerHelper;

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

            yield return Wait.UntilNextFrame(); // Or just 'yield break;' if you don't want to wait
        }
    }
}
