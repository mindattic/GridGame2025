using Assets.Scripts.Libraries;
using System.Collections;
using g = Assets.Helpers.GameHelper;
using scene = Assets.Helpers.SceneHelper;

namespace Assets.Scripts.Sequences
{
    /// <summary>
    /// Plays the defeat SFX then routes to VictoryScreen to award XP.
    /// Disables player input while sequence runs.
    /// </summary>
    public class BattleLostSequence : SequenceEvent
    {
        public override IEnumerator ProcessRoutine()
        {
            // Disable input
            g.InputManager.InputMode = InputMode.None;

            g.AudioManager.Play("Defeat");
            var sfx = SoundEffectLibrary.SoundEffects.ContainsKey("Defeat") ? SoundEffectLibrary.SoundEffects["Defeat"] : null;
            if (sfx != null)
                yield return Wait.For(sfx.length);
            // Route to Victory screen so XP is still awarded on defeat
            scene.Fade.ToVictoryScreen();
        }
    }
}
