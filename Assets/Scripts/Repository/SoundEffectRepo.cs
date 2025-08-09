using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public static class SoundEffectRepo
{
    private static Dictionary<string, AudioClip> soundEffects;

    public static Dictionary<string, AudioClip> SoundEffects
    {
        get
        {
            if (soundEffects == null)
                Load();
            return soundEffects;
        }
    }

    private static void Load()
    {
        soundEffects = new Dictionary<string, AudioClip>
        {
            { "Heal",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Click") },
            { "Click",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Click") },
            { "Death",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Death") },
            { "Move1",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move1") },
            { "Move2",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move2") },
            { "Move3",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move3") },
            { "Move4",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move4") },
            { "Move5",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move5") },
            { "Move6",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Move6") },
            { "NextTurn",     AssetHelper.LoadAsset<AudioClip>("SoundEffects/NextTurn") },
            { "PlayerGlow",   AssetHelper.LoadAsset<AudioClip>("SoundEffects/PlayerGlow") },
            { "Portrait",     AssetHelper.LoadAsset<AudioClip>("SoundEffects/Portrait") },
            { "Rumble",       AssetHelper.LoadAsset<AudioClip>("SoundEffects/Rumble") },
            { "Slash1",       AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash1") },
            { "Slash2",       AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash2") },
            { "Slash3",       AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash3") },
            { "Slash4",       AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash4") },
            { "Slash5",       AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash5") },
            { "Slash6",       AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash6") },
            { "Slash7",       AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slash7") },
            { "Slide",        AssetHelper.LoadAsset<AudioClip>("SoundEffects/Slide") }
        };
    }

    /// <summary>
    /// Retrieves a sound effect by key.
    /// </summary>
    public static AudioClip Get(string key)
    {
        if (SoundEffects.TryGetValue(key, out var clip))
            return clip;

        Debug.LogError($"SoundEffect '{key}' not found in SoundEffectRepo.");
        return null;
    }
}
