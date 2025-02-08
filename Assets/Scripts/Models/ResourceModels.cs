using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{

    public static class ResourceFolder
    {
        public static string Backgrounds = "Backgrounds";
        public static string Portraits = "Portraits";
        public static string SoundEffects = "SoundEffects";
        public static string MusicTracks = "MusicTracks";
        public static string Materials = "Materials";
        public static string Seamless = "Seamless";
        public static string Sprites = "Sprites";
        public static string Textures = "Textures";
        public static string WeaponTypes = "Sprites/WeaponTypes";
        public static string VisualEffects = "VisualEffects";
    }

    [Serializable]
    public class ResourceItem<T>
    {
        public T Value;                     
        public List<ResourceParameter> Parameters = new List<ResourceParameter>();
    }

    [Serializable]
    public class ResourceParameter
    {
        public string Key;  
        public string Value;
    }

    [Serializable]
    public class ResourceParameterList
    {
        public List<ResourceParameter> Parameters = new List<ResourceParameter>();
    }

}
