using System.Collections;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class EnemyStartAction : TurnAction
    {
        // Shortcut property to access the TurnManager instance.
        protected TurnManager turnManager => GameManager.instance.turnManager;

        public EnemyStartAction()
        {
        }

        public override IEnumerator Execute()
        {
            // Ensure this action only runs during enemy turns.
            if (!turnManager.isEnemyTurn)
                yield break;

            // (Optional) Log or perform any setup needed at the very start of the enemy turn.
            Debug.Log("EnemyStartAction executing: preparing enemy movement.");

            // Add the EnemyMoveAction to the Move phase.
            turnManager.AddActionToPhase(TurnPhase.Move, new EnemyMoveAction());

            // Switch the current phase to Move so that the EnemyMoveAction is processed next.
            turnManager.CurrentPhase = TurnPhase.Move;

            // Yield return null (or any brief wait) to allow the phase change to propagate.
            yield return null;
        }
    }
}
