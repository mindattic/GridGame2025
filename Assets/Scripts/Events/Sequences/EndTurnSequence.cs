using System.Collections;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Ends the current team's turn, switches to the next team,
    /// and starts their turn sequence.
    /// </summary>
    public class EndTurnSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            // Optional pacing before turn change
            yield return Wait.UntilNextFrame();

            // Swap sides
            g.TurnManager.NextTurn();

            // Start the next side's sequence and RUN it now
            if (g.TurnManager.isEnemyTurn)
            {
                g.SequenceManager.Add(new EnemyStartSequence());
                g.SequenceManager.TriggerExecute();
            }
            else if (g.TurnManager.isHeroTurn)
            {
                g.SequenceManager.Add(new HeroStartSequence());
                g.SequenceManager.TriggerExecute();
            }
        }
    }
}
