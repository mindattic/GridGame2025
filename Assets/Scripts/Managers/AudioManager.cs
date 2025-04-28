using Game.Behaviors;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Quick Reference Properties
    protected AudioSource soundSource => GameManager.instance.soundSource;


    public void Play(string sfx)
    {
        var soundEffect = SoundEffectRepo.SoundEffects[sfx];
        if (soundEffect == null)
        {
            Debug.LogError($@"Sound Effect `{sfx}` was not found.");
            return;
        }

        soundSource.PlayOneShot(soundEffect);
    }


}
