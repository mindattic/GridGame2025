using System.Collections;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Events
{
    public class HideTargetIndicatorSequence : SequenceEvent
    {
        public override IEnumerator Execute()
        {
            g.TargetIndicator.Hide();
            g.InputManager.inputMode = InputMode.HeroTurn;
            yield return null;
        }
    }


}
