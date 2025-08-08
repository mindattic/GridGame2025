// --- File: Assets/Scripts/Events/Sequences/EndTurnSequence.cs ---
using System.Collections;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Ends the current team's turn and flips to the next team.
    /// Responsibility for enqueuing the next side's start sequence
    /// belongs to TurnManager.EnterCurrentSide via TurnManager.NextTurn.
    /// </summary>
    public class EndTurnSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            // Small pacing if you want any end-of-turn VFX to finish
            yield return Wait.UntilNextFrame();

            // Flip side. TurnManager.NextTurn will enqueue the appropriate
            // start sequence and trigger execution. Do not enqueue here.
            g.TurnManager.NextTurn();

            // Nothing else to do in this sequence
            yield break;
        }
    }
}
