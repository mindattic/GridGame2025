using Game.Behaviors;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Properties
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected LogManager logManager => GameManager.instance.logManager;
    protected AudioSource soundSource => GameManager.instance.soundSource;
    #endregion


    public void Play(string sfx)
    {
        var soundEffect = resourceManager.SoundEffect(sfx).Value;
        if (soundEffect == null)
        {
            logManager.Error($@"Sound Effect ""{sfx}"" was not found.");
            return;
        }

        soundSource.PlayOneShot(soundEffect);
    }


}
