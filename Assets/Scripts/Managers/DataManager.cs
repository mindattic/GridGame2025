using Assets.Scripts.Models;
using Game.Behaviors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    }

    public string Name;
    public string Description;
    public string CompletionCondition;
    public int CompletionValue;
    public List<StageActor> Actors;
    public List<StageDottedLine> DottedLines;
}


[Serializable]
public class StageActor
{
    public StageActor() { }

    public StageActor(StageActor other)
    {
        Character = other.Character;
        Team = other.Team;
        Location = other.Location;
    }

    public string Character;
    public string Team;
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


public class DataManager : MonoBehaviour
{
    protected LogManager logManager => GameManager.instance.logManager;

    public static class Resource
    {
        public static string Actors = "Actors";
        public static string VisualEffects = "VisualEffects";
        public static string Stages = "Stages";
    }

    public List<ActorData> Actors = new List<ActorData>();
    public List<VisualEffectData> VisualEffects = new List<VisualEffectData>();
    public List<StageData> Stages = new List<StageData>();

    public List<T> ParseJson<T>(string resource)
    {
        string filePath = $"Data/{resource}";

        Debug.Log(filePath);
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);

        if (jsonFile == null)
        {
            logManager.Error($"File {filePath} not found in Resources.");
            return null;
        }

        var collection = JsonConvert.DeserializeObject<JsonWrapper<T>>(jsonFile.text);
        return collection.Items;
    }

    public void Initialize()
    {
        Actors = ParseJson<ActorData>(Resource.Actors);
        VisualEffects = ParseJson<VisualEffectData>(Resource.VisualEffects);
        Stages = ParseJson<StageData>(Resource.Stages);
    }

    public ActorStats GetStats(Character character)
    {
        var data = Actors.Where(x => x.Character == character.ToString()).FirstOrDefault().Stats;
        if (data == null)
            logManager.Error($"Unable to retrieve actor stats for `{character}`");


        return new ActorStats(data); //Return a new copy instead of a shared reference
        //return data;
    }


    public ThumbnailSettings GetThumbnailSetting(Character character)
    {
        var data = Actors.Where(x => x.Character == character.ToString()).FirstOrDefault().ThumbnailSettings;
        if (data == null)
            logManager.Error($"Unable to retrieve thumnail settings for `{name}`");

        return new ThumbnailSettings(data); //Return a new copy instead of a shared reference
        //return data;
    }

    public ActorDetails GetDetails(Character character)
    {
        var data = Actors.Where(x => x.Character == character.ToString()).FirstOrDefault().Details;
        if (data == null)
            logManager.Error($"Unable to retrieve actor details for `{character}`");

        return new ActorDetails(data);
    }

    public VisualEffectData GetVisualEffect(string name)
    {
        var data = VisualEffects.Where(x => x.Name == name).FirstOrDefault();
        if (data == null)
            logManager.Error($"Unable to retrieve visual effect for `{name}`");

        return new VisualEffectData(data);
    }

    public StageData GetStage(string name)
    {
        var data = Stages.Where(x => x.Name == name).FirstOrDefault();
        if (data == null)
            logManager.Error($"Unable to retrieve stage for `{name}`");

        return new StageData(data);
    }

}
