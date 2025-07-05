using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Performs any start-of-turn animation or logic for the enemy team.
    /// Does not advance phase or add move sequences—TurnManager handles that.
    /// </summary>
    public class EnemyStartSequence : SequenceEvent
    {
        #region Game Properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        //protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;
        #endregion

        public override IEnumerator Execute()
        {
            // Only run during enemy turns.
            if (!turnManager.isEnemyTurn)
                yield break;

            // (Optional) Do any enemy team "start" animation, camera, dialog, etc. here.
            // Example: yield return new WaitForSeconds(0.5f);

            // Do NOT add EnemyMoveSequence or change phase.
            // TurnManager will enqueue moves for all ready enemies in Move phase.

            yield return Wait.UntilNextFrame(); // Or just 'yield break;' if you don't want to wait
        }
    }
}
