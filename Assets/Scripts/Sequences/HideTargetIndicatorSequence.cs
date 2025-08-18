using System.Collections;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    public class HideTargetIndicatorSequence : SequenceEvent
    {
        public override IEnumerator ProcessRoutine()
        {
            g.Actors.TargetActor = null;
            g.TargetIndicator.Hide();
            g.InputManager.InputMode = InputMode.PlayerTurn;
            yield return Wait.None();
        }
    }


}
