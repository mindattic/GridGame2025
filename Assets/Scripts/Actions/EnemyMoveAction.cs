using System.Collections;
using System.Linq;
using Action = Assets.Scripts.Models.PhaseAction;

namespace Assets.Scripts.Models
{
    public class EnemyMoveAction : Action
    {
       //Quick Reference Properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected ActionManager actionManager => GameManager.instance.actionManager;
        protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;

        public EnemyMoveAction()
        {
        }

        public override IEnumerator Execute()
        {
            //Only proceed if it is the enemy's turn.
            if (!turnManager.isEnemyTurn)
                yield break;

            //Find all enemies that are ready (active, alive and with full AP).
            var readyEnemies = enemies.ToList().Where(x => x.isPlaying && x.hasMaxAP).ToList();



            if (readyEnemies.Count > 0)
            {
                //Wait for a predetermined waitDuration before enemy movement starts.
                yield return Wait.For(Intermission.Before.Enemy.Move);

                //For each ready enemy, calculate its attack strategy and movement it to its destination.
                foreach (var enemy in readyEnemies)
                {
                    enemy.CalculateAttackStrategy();
                    yield return enemy.movement.MoveTowardDestination();
                }

                //After moving, add the enemy attack action.
                actionManager.Add(new EnemyAttackAction());
                turnManager.SetPhase(TurnPhase.Attack);
            }
            else
            {
                //If no enemy is ready, immediately advance the turn (back to player turn).
                turnManager.NextTurn();
            }
        }
    }
}
