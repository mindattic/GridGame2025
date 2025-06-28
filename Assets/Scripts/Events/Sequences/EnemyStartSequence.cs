using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class EnemyStartSequence : SequenceEvent
    {
       //Quick Reference Properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;

        public EnemyStartSequence()
        {
        }

        public override IEnumerator Execute()
        {
            // Ensure this action only runs during enemy turns.
            if (!turnManager.isEnemyTurn)
                yield break;

            // (Optional) Log or perform any setup needed at the very start of the enemy turn.
            //Debug.Log("EnemyStartAction executing: preparing enemy movement.");

            sequenceManager.Add(new EnemyMoveSequence());
            turnManager.SetPhase(TurnPhase.Move);

            // Yield return null (or any brief wait) to allow the phase change to propagate.
            yield return Wait.UntilNextFrame();
        }
    }
}
