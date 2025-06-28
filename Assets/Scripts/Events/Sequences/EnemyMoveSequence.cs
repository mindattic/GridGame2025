using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Events
{
    public class EnemyMoveSequence : SequenceEvent
    {
        //Quick Reference Properties
        protected TurnManager turnManager => GameManager.instance.turnManager;
        protected SequenceManager sequenceManager => GameManager.instance.sequenceManager;
        protected List<ActorInstance> actors = GameManager.instance.actors;
        protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;

        //Constructor
        public EnemyMoveSequence() { }

        public override IEnumerator Execute()
        {
            //Only proceed if it is the enemy's turn.
            if (!turnManager.isEnemyTurn)
                yield break;

            //Find all enemies that are ready (active, alive and with full AP).
            var readyEnemies = enemies.ToList().Where(x => x.isPlaying && x.hasMaxAP).ToList();
            if (readyEnemies.Count < 1)
            {
                turnManager.NextTurn();
                yield break;
            }

            //actors.ForEach(x => x.sortingOrder = SortingOrder.Default);

            //Wait for a predetermined waitDuration before enemy movement starts.
            yield return Wait.For(Intermission.Before.Enemy.Move);

            //For each ready enemy, calculate its attackResult strategy and movement it to its destination.
            foreach (var enemy in readyEnemies)
            {
                enemy.CalculateAttackStrategy();
                yield return enemy.movement.MoveTowardDestination();
            }

            //After moving, add the enemy attackResult action.
            sequenceManager.Add(new DirectAttackSequence());
            turnManager.SetPhase(TurnPhase.Attack);
        }
    }
}
