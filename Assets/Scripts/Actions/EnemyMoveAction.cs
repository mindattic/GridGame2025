using System.Collections;
using System.Linq;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class EnemyMoveAction : TurnAction
    {
        //External properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
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
                //Wait for a predetermined delay before enemy movement starts.
                yield return Wait.For(Intermission.Before.Enemy.Move);

                //For each ready enemy, calculate its attack strategy and move it to its destination.
                foreach (var enemy in readyEnemies)
                {
                    enemy.CalculateAttackStrategy();
                    yield return enemy.move.MoveTowardDestination();
                }

                //After moving, add the enemy attack action.
                turnManager.AddAction(new EnemyAttackAction());
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
