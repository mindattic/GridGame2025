using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class AudioManager : MonoBehaviour
{
    public void Play(string sfx)
    {
        var soundEffect = SoundEffectLibrary.SoundEffects[sfx];
        if (soundEffect == null)
        {
            Debug.LogError($@"Sound Effect `{sfx}` was not found.");
            return;
        }

        g.SoundSource.PlayOneShot(soundEffect);
    }


}
