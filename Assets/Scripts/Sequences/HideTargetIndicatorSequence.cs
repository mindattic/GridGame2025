using Assets.Helper;
using System.Collections;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    public class HideTargetIndicatorSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            g.TargetIndicator.Hide();
            g.InputManager.inputMode = InputMode.Player;
            yield return Wait.None();
        }
    }


}
