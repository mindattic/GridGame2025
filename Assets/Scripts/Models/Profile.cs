using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Game.Models.Profile
{
    [Serializable]
    public class Profile
    {
        // Fields
        public string Key;
        public string Folder;
        public List<SaveState> SaveStates = new List<SaveState>();
        public ProfileSettings Settings;
        public SaveState CurrentSave;

        // Properties
        public bool HasSaves => SaveStates.Any();
        public bool HasCurrentSave => CurrentSave != null;
        public SaveState LatestSave => HasSaves ? SaveStates.OrderByDescending(x => x.Timestamp).First() : null;

        public Profile() { }
    }

    [Serializable]
    public class ProfileSettings
    {
        public float ActorPanMultiplier;
        public float GameSpeed;

        public ProfileSettings() { }

        public ProfileSettings(ProfileSettings other)
        {
            this.ActorPanMultiplier = other.ActorPanMultiplier;
            this.GameSpeed = other.GameSpeed;
        }
    }

    [Serializable]
    public class SaveState
    {
        //public string Folder;
        public string FileName;
        public DateTime Timestamp;
        public int Index;
        public GlobalSaveData Global;
        public StageSaveData Stage;
        public PartySaveData Party;
        
        public SaveState() { }

        public SaveState(int index, DateTime timestamp, GlobalSaveData global, StageSaveData stage, PartySaveData party)
        {
            Index = index;
            Timestamp = timestamp;
            //Folder = Path.Combine(ProfileRepo.instance.CurrentProfile.Folder, "Saves");
            FileName = $"Save{index:D3}.json";
            Global = global;
            Stage = stage;
            Party = party;
        }

        //Copy constructor
        public SaveState(SaveState other)
        {
            this.Index = other.Index;
            this.Timestamp = other.Timestamp;
            //this.Folder = other.Folder;
            this.FileName = other.FileName;
            this.Global = new GlobalSaveData(other.Global);
            this.Stage = new StageSaveData(other.Stage);
            this.Party = new PartySaveData(other.Party);
        }
    }

    [Serializable]
    public class GlobalSaveData
    {
        public int TotalCoins;

        public GlobalSaveData() { }
        public GlobalSaveData(GlobalSaveData other)
        {
            this.TotalCoins = other.TotalCoins;
        }
    }

    [Serializable]
    public class StageSaveData
    {
        public string CurrentStage;
        public int CurrentWave;

        public StageSaveData() { }
        public StageSaveData(StageSaveData other)
        {
            this.CurrentStage = other.CurrentStage;
            this.CurrentWave = other.CurrentWave;
        }
    }

    [Serializable]
    public class PartySaveData
    {
        public List<StageActor> HeroActors = new List<StageActor>();

        public PartySaveData() { }
        public PartySaveData(PartySaveData other)
        {
            this.HeroActors = other.HeroActors;
        }
    }
}
