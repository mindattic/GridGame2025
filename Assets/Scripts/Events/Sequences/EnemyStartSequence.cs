using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class EnemyStartSequence : SequenceEvent
    {
        #region Game Properies
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;
        #endregion

        public override IEnumerator Execute()
        {
            // Ensure this animate only runs during enemy turns.
            if (!turnManager.isEnemyTurn)
                yield break;

            // (Optional) Log or perform any setup needed at the very start of the enemy turn.
            //Debug.Log("EnemyStartAction executing: preparing enemy move.");

            sequenceManager.Add(new EnemyMoveSequence());
            turnManager.SetPhase(TurnPhase.Move);

            // Yield return null (or any brief wait) to allow the phase change to propagate.
            yield return Wait.UntilNextFrame();
        }
    }
}
