using Assets.Scripts.Models;
using Game.Behaviors;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    protected LogManager logManager => GameManager.instance.logManager;

    public static class Resource
    {
        public static string Actors = "Actors";
        public static string Stages = "Stages";
        public static string TrailEffects = "TrailEffects";
        public static string VisualEffects = "VisualEffects";
    }

    public List<ActorData> Actors = new List<ActorData>();
    public List<StageData> Stages = new List<StageData>();
    public List<TrailData> TrailEffects = new List<TrailData>();
    public List<VFXData> VisualEffects = new List<VFXData>();

    public List<T> ParseJson<T>(string resource)
    {
        string filePath = $"Data/{resource}";
        //Debug.Log(filePath);
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
        Stages = ParseJson<StageData>(Resource.Stages);
        TrailEffects = ParseJson<TrailData>(Resource.TrailEffects);
        VisualEffects = ParseJson<VFXData>(Resource.VisualEffects);
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

        return new ActorDetails(data); //Return a new copy instead of a shared reference
    }

    public StageData GetStage(string name)
    {
        var data = Stages.Where(x => x.Name == name).FirstOrDefault();
        if (data == null)
            logManager.Error($"Unable to retrieve stage for `{name}`");

        return new StageData(data); //Return a new copy instead of a shared reference
    }
    public TrailData GetTrailEffect(string name)
    {
        var data = TrailEffects.Where(x => x.Name == name).FirstOrDefault();
        if (data == null)
            logManager.Error($"Unable to retrieve trailInstance effect for `{name}`");

        return new TrailData(data); //Return a new copy instead of a shared reference
    }

    public VFXData GetVisualEffect(string name)
    {
        var data = VisualEffects.Where(x => x.Name == name).FirstOrDefault();
        if (data == null)
            logManager.Error($"Unable to retrieve visual effect for `{name}`");

        return new VFXData(data); //Return a new copy instead of a shared reference
    }

}
