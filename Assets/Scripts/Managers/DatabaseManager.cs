
//using Assets.Scripts.Entities;
//using Assets.Scripts.Models;
//using Game.Behaviors;
//using SQLiteDatabase;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;


//namespace Game.Manager
//{
//   public class DatabaseManager : MonoBehaviour
//   {
//       public static class Schema
//       {
//           public const string DBName = "MyDatabase.db";

//           public static class Table
//           {

//               public static string Actor = "Actor";
//           }
//       }

//       public static class Queries
//       {
//           public static class Load
//           {
//               public static class Actor
//               {
//                   public static string Entities = " SELECT a.Name, a.Description, s.Level, s.MaxHp, s.Strength, s.Vitality, s.Agility, s.Speed, s.Luck, t.Width AS ThumbnailWidth, t.Width AS ThumbnailHeight, t.OffsetX AS ThumbnailX, t.OffsetY AS ThumbnailY FROM Actor a INNER JOIN ActorStats a_s ON (a.ActorId = a_s.ActorId) INNER JOIN Stats s ON (a_s.StatsId = s.StatsId) INNER JOIN ActorThumbnail a_t ON (a.ActorId = a_t.ActorId) INNER JOIN Thumbnail t ON (t.ThumbnailId = a_t.ThumbnailId)";



//               }
//           }
//       }

//       public static class Entities
//       {
//           public static List<ActorData> Actors = new List<ActorData>();
//       }


//       //Quick Reference Properties
//       protected LogManager logManager => GameManager.trailInstance.logManager;
//       #endregion

//       //Fields
//       public const bool autoOverwrite = true; //Used to reinstall app every load...
//       private SQLiteDB trailInstance = SQLiteDB.Instance;


//       void OnEnable()
//       {
//           SQLiteEventListener.onError += OnError;
//       }

//       void OnDisable()
//       {
//           SQLiteEventListener.onError -= OnError;
//       }

//       void OnError(string err)
//       {
//           logManager.Error(err);
//       }

//       void OnApplicationQuit()
//       {
//           trailInstance.Dispose();
//       }

//       //Method which is used for initialization tasks that need to occur before the game starts 
//       private void Awake()
//       {
//           trailInstance.DBLocation = Application.persistentDataPath;
//           trailInstance.DBName = Schema.DBName;

//           //Update if this is the first load of the application
//           if (autoOverwrite || !trailInstance.Exists)
//               trailInstance.CreateDatabase(trailInstance.DBName, isOverWrite: true);

//           var isConnected = trailInstance.ConnectToDefaultDatabase(trailInstance.DBName, loadFresh: true);

//           if (!isConnected)
//               throw new UnityException($"Failed to connect to database: {trailInstance.DBName}");

//           Initialize(); //TODO: Initialize data based on current stage???...
//       }


//       //TODO:
//       //Come up with a way to retrieve records piecemeal so
//       //that entire database tables don't have to be
//       //downloaded for a small subset of data, e.g:
//       //StageData 05: ["Slime", "Scorpion", "Bat", "Yeti"]


//       void Initialize()
//       {
//           DBReader reader;

//           Entities.Actors.Clear();

//           //reader = trailInstance.GetAllData(Schema.Table.Actor);
//           reader = trailInstance.ExecuteReader(Queries.Load.Actor.Entities);
//           while (reader != null && reader.Read())
//           {
//               var x = new ActorData()
//               {
//                   Name = reader.GetStringValue("Name"),
//                   Description = reader.GetStringValue("Description"),
//               };

//               x.Stats = new ActorStats()
//               {
//                   HP = reader.GetFloatValue("MaxHp"),
//                   MaxHP = reader.GetFloatValue("MaxHp"),
//                   Strength = reader.GetFloatValue("Strength"),
//                   Vitality = reader.GetFloatValue("Vitality"),
//                   Agility = reader.GetFloatValue("Agility"),
//                   Speed = reader.GetFloatValue("Speed"),
//                   Luck = reader.GetFloatValue("Luck"),
//               };

//               x.thumbnailSettings = new ThumbnailSettings()
//               {
//                   Width = reader.GetIntValue("ThumbnailWidth"),
//                   Height = reader.GetIntValue("ThumbnailHeight"),
//                   OffsetX = reader.GetIntValue("ThumbnailX"),
//                   OffsetY = reader.GetIntValue("ThumbnailY"),
//               };

//               //x.Rarity = new Rarity()
//               //{
//               //   Name = reader.GetStringValue("RarityName"),
//               //   Color = ColorHelper.Solid.White,
//               //};


//               Entities.StageActors.SpawnActor(x);
//               logManager.Info(JsonConvert.Serialize(x));
//           };
//       }

//       public ActorStats GetStats(string name)
//       {
//           return Entities.Actors.Where(x => x.Name == name).FirstOrDefault().Stats;
//       }


//       public ThumbnailSettings GetThumbnailSetting(string name)
//       {
//           return Entities.Actors.Where(x => x.Name == name).FirstOrDefault().thumbnailSettings;
//       }

//   }
//}