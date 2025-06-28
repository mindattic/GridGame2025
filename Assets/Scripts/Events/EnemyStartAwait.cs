using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class EnemyStartAwait : AwaitEvent
    {
       //Quick Reference Properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected EventManager eventManager => GameManager.instance.eventManager;

        public EnemyStartAwait()
        {
        }

        public override IEnumerator Execute()
        {
            // Ensure this action only runs during enemy turns.
            if (!turnManager.isEnemyTurn)
                yield break;

            // (Optional) Log or perform any setup needed at the very start of the enemy turn.
            //Debug.Log("EnemyStartAction executing: preparing enemy movement.");

            eventManager.Add(new EnemyMoveAwait());
            turnManager.SetPhase(TurnPhase.Move);

            // Yield return null (or any brief wait) to allow the phase change to propagate.
            yield return Wait.UntilNextFrame();
        }
    }
}
