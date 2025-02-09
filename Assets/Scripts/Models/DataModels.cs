using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string Character;
        public string Description;
        public ActorStats Stats;
        public ThumbnailSettings ThumbnailSettings;
        public ActorDetails Details;
    }


    [Serializable]
    public class VisualEffectData
    {
        public VisualEffectData() { }

        public VisualEffectData(VisualEffectData other)
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
        public string RelativeOffset;
        public string AngularRotation;
        public string RelativeScale;
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

    [Serializable]
    public class StageData
    {
        public StageData() { }

        public StageData(StageData other)
        {
            Name = other.Name;
            Description = other.Description;
            CompletionCondition = other.CompletionCondition;
            CompletionValue = other.CompletionValue;
            Actors = other.Actors != null ? new List<StageActor>(other.Actors) : new List<StageActor>();
            DottedLines = other.DottedLines != null ? new List<StageDottedLine>(other.DottedLines) : new List<StageDottedLine>();
            Tutorials = other.Tutorials != null ? new List<string>(other.Tutorials) : new List<string>();
        }

        public string Name;
        public string Description;
        public string CompletionCondition;
        public int CompletionValue;
        public string NextStage = "Stage 2";
        public List<StageActor> Actors;
        public List<StageDottedLine> DottedLines;
        public List<string> Tutorials;

    }


    [Serializable]
    public class StageActor
    {
        public StageActor() { }

        public StageActor(StageActor other)
        {
            Character = other.Character;
            Team = other.Team;
            SpawnTurn = other.SpawnTurn;
            Location = other.Location;
        }

        public string Character;
        public string Team;
        public int SpawnTurn;
        public string Location;
    }

    [Serializable]
    public class StageDottedLine
    {
        public StageDottedLine() { }

        public StageDottedLine(StageDottedLine other)
        {
            Segment = other.Segment;
            Location = other.Location;
        }

        public string Segment;
        public string Location;
    }
}
