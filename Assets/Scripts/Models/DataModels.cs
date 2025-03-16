using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class JsonWrapper<T>
    {
        public List<T> Items;
    }

    [Serializable]
    public class ActorData
    {
        public ActorData() { }

        public ActorData(ActorData other)
        {
            Character = other.Character;
            Description = other.Description;
            Stats = other.Stats != null ? new ActorStats(other.Stats) : new ActorStats();
            ThumbnailSettings = other.ThumbnailSettings != null ? new ThumbnailSettings(other.ThumbnailSettings) : new ThumbnailSettings();
            Details = other.Details != null ? new ActorDetails(other.Details) : new ActorDetails();
        }

        public Character Character;
        public string Description;
        public ActorStats Stats;
        public ThumbnailSettings ThumbnailSettings;
        public ActorDetails Details;
    }


    [Serializable]
    public class VFXData
    {
        public VFXData() { }

        public VFXData(VFXData other)
        {
            Name = other.Name;
            RelativeOffset = other.RelativeOffset;
            AngularRotation = other.AngularRotation;
            RelativeScale = other.RelativeScale;
            Delay = other.Delay;
            Duration = other.Duration;
            IsLoop = other.IsLoop;
        }

        public string Name;
        public Vector3 RelativeOffset;
        public Vector3 AngularRotation;
        public Vector3 RelativeScale;
        public float Delay;
        public float Duration;
        public bool IsLoop;
    }



    [Serializable]
    public enum StageCompletionCondition
    {
        DefeatAllEnemies,
        CollectCoins,
        SurviveTurns
    }



  
}
