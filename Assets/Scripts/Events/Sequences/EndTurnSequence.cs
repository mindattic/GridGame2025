// --- File: Assets/Scripts/Events/Sequences/EndTurnSequence.cs ---
using System.Collections;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Ends the current team turn and flips to the next team.
    /// TurnManager.NextTurn enqueues the appropriate side-start sequence.
    /// </summary>
    public class EndTurnSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            // Optional tiny pacing for any end-of-turn visuals
            yield return Wait.UntilNextFrame();

            // Flip sides and let TurnManager enqueue the next side's start sequence
            g.TurnManager.NextTurn();

            yield break;
        }
    }
}
