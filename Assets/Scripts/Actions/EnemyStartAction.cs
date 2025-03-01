using System.Collections;
using Action = Assets.Scripts.Models.PhaseAction;

namespace Assets.Scripts.Models
{
    public class EnemyStartAction : Action
    {
       //Quick Reference Properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected ActionManager actionManager => GameManager.instance.actionManager;

        public EnemyStartAction()
        {
        }

        public override IEnumerator Execute()
        {
            // Ensure this action only runs during enemy turns.
            if (!turnManager.isEnemyTurn)
                yield break;

            // (Optional) Log or perform any setup needed at the very start of the enemy turn.
            //Debug.Log("EnemyStartAction executing: preparing enemy movement.");

            actionManager.Add(new EnemyMoveAction());
            turnManager.SetPhase(TurnPhase.Move);

            // Yield return null (or any brief wait) to allow the phase change to propagate.
            yield return Wait.UntilNextFrame();
        }
    }
}
